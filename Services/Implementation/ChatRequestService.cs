using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.Implementation
{
    public class ChatRequestService : IChatRequestService
    {
        private readonly IChatQueue _mainQueue;
        private readonly IChatQueue _overflowQueue;
        private readonly ITeamManagementService _teamManagementService;
        private readonly IOfficeHoursService _officeHoursService;
        private readonly ILogger<ChatRequestService> _logger;
        private readonly Dictionary<Guid, ChatSession> _sessions = new();
        private readonly object _lock = new();

        public ChatRequestService(
            IChatQueue mainQueue,
            IChatQueue overflowQueue,
            ITeamManagementService teamManagementService,
            IOfficeHoursService officeHoursService,
            ILogger<ChatRequestService> logger)
        {
            _mainQueue = mainQueue;
            _overflowQueue = overflowQueue;
            _teamManagementService = teamManagementService;
            _officeHoursService = officeHoursService;
            _logger = logger;
        }

        public async Task<ChatResponse> InitiateChatAsync(ChatRequest request)
        {
            var session = new ChatSession
            {
                SessionId = Guid.NewGuid(),
                UserId = request.UserId,
                Messsage = request.Message,
                Status = ChatSessionStatus.Queued,
                CreatedAt = DateTime.Now,
                LastPolledAt = DateTime.Now
            };

            // Calculate Capacities
            var mainMaxQueue = _teamManagementService.CalculateMaxQueueLength(false);
            var isOfficeHours = _officeHoursService.IsOfficeHours();

            // Check Main Queue
            if (!_mainQueue.IsFull(mainMaxQueue))
            {
                _mainQueue.Enqueue(session);

                lock (_lock)
                {
                    _sessions[session.SessionId] = session;
                }

                _logger.LogInformation($"Chat session {session.SessionId} initiated and added to main queue.");

                return new ChatResponse
                {
                    Success = true,
                    SessionId = session.SessionId,
                    Status = "OK",
                    Message = "Chat session created and queued"
                };
            }

            // Main queue is full, check overflow
            if (isOfficeHours)
            {
                var overflowMaxQueue = _teamManagementService.GetOverflowTeam().CalculateMaxQueuesLength();

                if (!_overflowQueue.IsFull(overflowMaxQueue))
                {
                    _overflowQueue.Enqueue(session);

                    lock (_lock)
                    {
                        _sessions[session.SessionId] = session;
                    }

                    _logger.LogInformation($"Chat session {session.SessionId} initiated and queued to overflow queue.");

                    return new ChatResponse
                    {
                        Success = true,
                        SessionId = session.SessionId,
                        Status = "OK",
                        Message = "Chat session created and queued to overflow queue"
                    };
                }
            }

            // Both queues are full or oveerflow not available
            _logger.LogWarning($"Chat request refused for user {request.UserId}. Both queues are full or overflow not available.");

            return new ChatResponse
            {
                Success = false,
                Status = "REFUSED",
                Message = "All support agents are currently busy. Please try again later."
            };
        }

        public async Task<ChatResponse> GetChatStatusAsync(Guid sessionId)
        {
            lock (_lock)
            {
                if (_sessions.TryGetValue(sessionId, out var session))
                {
                    session.LastPolledAt = DateTime.Now;
                    session.MissedPollCount = 0;

                    var statusMessage = session.Status switch
                    {
                        ChatSessionStatus.Queued => "Your chat is in queue",
                        ChatSessionStatus.Assigned => $"Assigned to agent {session.AssignedAgentId}",
                        ChatSessionStatus.Active => "Chat is active",
                        ChatSessionStatus.Inactive => "Session marked as inactive",
                        ChatSessionStatus.Completed => "Chat completed",
                        _ => "Unknown status"
                    };

                    return new ChatResponse
                    {
                        Success = true,
                        SessionId = sessionId,
                        Status = session.Status.ToString(),
                        Message = statusMessage
                    };
                }
            }

            return new ChatResponse
            {
                Success = false,
                Status = "NOT_FOUND",
                Message = "Chat session not found"
            };
        }

        public ChatSession GetSession(Guid sessionId)
        {
            lock (_lock)
            {
                return _sessions.TryGetValue(sessionId, out var session) ? session : null;
            }
        }

        public void UpdateSession(ChatSession session)
        {
            lock (_lock)
            {
                _sessions[session.SessionId] = session;                
            }
        }
    }
}