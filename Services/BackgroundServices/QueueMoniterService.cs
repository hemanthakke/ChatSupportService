using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.BackgroundServices
{
    public class QueueMoniterService : BackgroundService
    {
        private readonly IChatQueue _mainQueue;
        private readonly IChatQueue _overflowQueue;
        private readonly IChatRequestService _chatRequestService;
        private readonly ILogger<QueueMoniterService> _logger;
        private const int PollIntervalSeconds = 1;
        private const int MaxMissedPolls = 3;

        public QueueMoniterService(
            IChatQueue mainQueue,
            IChatQueue overflowQueue,
            IChatRequestService chatRequestService,
            ILogger<QueueMoniterService> logger)
        {
            _mainQueue = mainQueue;
            _overflowQueue = overflowQueue;
            _chatRequestService = chatRequestService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queue Monitor Service is started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    MonitorQueue(_mainQueue);
                    MonitorQueue(_overflowQueue);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Queue Monitor Service");
                }

                await Task.Delay(TimeSpan.FromSeconds(PollIntervalSeconds), stoppingToken);
            }
        }

        private void MonitorQueue(IChatQueue queue)
        {
            var sessions = queue.GetAll();
            var now = DateTime.Now;

            foreach (var session in sessions)
            { 
                var timeSinceLastPoll = (now - session.LastPolledAt).TotalSeconds;

                if (timeSinceLastPoll > PollIntervalSeconds)
                {
                    session.MissedPollCount++;

                    if (session.MissedPollCount >= MaxMissedPolls)
                    { 
                        session.Status = ChatSessionStatus.Inactive;
                        queue.Remove(session.SessionId);
                        _chatRequestService.UpdateSession(session);
                        _logger.LogWarning("Session {SessionId} marked as Inactive due to missed polls", session.SessionId);
                    }
                }
            }
        }
    }
}