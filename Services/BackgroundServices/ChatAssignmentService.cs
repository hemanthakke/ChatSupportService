using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.BackgroundServices
{
    public class ChatAssignmentService : BackgroundService
    {
        private readonly IChatQueue _mainQueue;
        private readonly IChatQueue _overflowQueue;
        private readonly ITeamManagementService _teamManagementService;
        private readonly IChatRequestService _chatRequestService;
        private readonly ILogger<ChatAssignmentService> _logger;

        public ChatAssignmentService(
            IChatQueue mainQueue,
            IChatQueue overflowQueue,
            ITeamManagementService teamManagementService,
            IChatRequestService chatRequestService,
            ILogger<ChatAssignmentService> logger)
        {
            _mainQueue = mainQueue;
            _overflowQueue = overflowQueue;
            _teamManagementService = teamManagementService;
            _chatRequestService = chatRequestService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Chat Assignment Service started...");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    AssignChatsFromQueue();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Chat Assignment Service");
                }
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            _logger.LogInformation("Chat Assignment Service stopped...");
        }

        private void AssignChatsFromQueue()
        {
            var availableAgents = _teamManagementService.GetAvailableAgents()
                .OrderBy(agent => agent.Seniority) // Junior agents get assigned first
                .ThenBy(agent => agent.CurrentChatsCount)
                .ToList();

            if (!availableAgents.Any())
                return;

            // First Main queue assignment
            AssignFromQueue(_mainQueue, availableAgents);

            // Then Overflow queue assignment
            AssignFromQueue(_overflowQueue, availableAgents);
        }

        private void AssignFromQueue(IChatQueue queue, List<Agent> availableAgents)
        {
            while (queue.Count > 0 && availableAgents.Any(agent => agent.CurrentChatsCount < agent.MaxConcurrency))
            {
                var session = queue.Peek();

                if (session == null)
                    break;

                // Find next available agent(round robin, junior first)
                var agent = availableAgents
                    .Where(agent => agent.CurrentChatsCount < agent.MaxConcurrency)
                    .OrderBy(agent => agent.Seniority)
                    .ThenBy(agent => agent.CurrentChatsCount)
                    .FirstOrDefault();

                if (agent != null)
                {
                    queue.Dequeue();
                    session.AssignedAgentId = agent.Id;
                    session.Status = ChatSessionStatus.Assigned;
                    agent.CurrentChatsCount++;

                    _chatRequestService.UpdateSession(session);
                    _logger.LogInformation($"Assigned chat session {session.SessionId} to agent {agent.Name} ({agent.Seniority}) (Current Chats: {agent.CurrentChatsCount}/{agent.MaxConcurrency})");
                }
                else
                {
                    break;
                }
            }
        }
    }
}