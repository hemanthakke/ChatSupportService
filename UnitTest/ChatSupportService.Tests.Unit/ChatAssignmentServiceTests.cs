using ChatSupportService.Models;
using ChatSupportService.Services.BackgroundServices;
using ChatSupportService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatSupportService.Tests.Unit
{
    public class ChatAssignmentServiceTests
    {
        private readonly Mock<IChatQueue> _mockMainQueue;
        private readonly Mock<IChatQueue> _mockOverflowQueue;
        private readonly Mock<ITeamManagementService> _mockTeamManagementService;
        private readonly Mock<IChatRequestService> _mockChatRequestService;
        private readonly Mock<ILogger<ChatAssignmentService>> _mockLogger;
        private readonly ChatAssignmentService _chatAssignmentService;

        public ChatAssignmentServiceTests()
        {
            _mockMainQueue = new Mock<IChatQueue>();
            _mockOverflowQueue = new Mock<IChatQueue>();
            _mockTeamManagementService = new Mock<ITeamManagementService>();
            _mockChatRequestService = new Mock<IChatRequestService>();
            _mockLogger = new Mock<ILogger<ChatAssignmentService>>();

            _chatAssignmentService = new ChatAssignmentService(
                    _mockMainQueue.Object,
                    _mockOverflowQueue.Object,
                    _mockTeamManagementService.Object,
                    _mockChatRequestService.Object,
                    _mockLogger.Object
            );
        }

        [Fact]
        public async Task CanExecuteAsyncTest()
        { 
            var cts = new CancellationTokenSource();
            var callCnt = 0;

            _mockTeamManagementService
                .Setup(x => x.GetAvailableAgents())
                .Returns(() =>
                {
                    callCnt++;
                    if (callCnt >= 3)
                    { 
                        cts.Cancel();
                    }
                    return new List<Agent>();
                });

            await _chatAssignmentService.StartAsync(cts.Token);
            await Task.Delay(3500); 
            await _chatAssignmentService.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task CanExecuteAsyncExceptionTest()
        {
            var cts = new CancellationTokenSource();
            var expectedException = new InvalidOperationException("Test exception");
            var callCnt = 0;

            _mockTeamManagementService
                .Setup(x => x.GetAvailableAgents())
                .Returns(() =>
                {
                    callCnt++;
                    if (callCnt == 1)
                    {
                        throw expectedException;
                    }

                    cts.Cancel();
                    return new List<Agent>();
                });

            await _chatAssignmentService.StartAsync(cts.Token);
            await Task.Delay(2500);
            await _chatAssignmentService.StopAsync(CancellationToken.None);
        }

        [Fact]
        public async Task CanAssignChatsFromQueueTest()
        {
            var agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Seniority = Seniority.Junior, CurrentChatsCount = 0 },
                new Agent { Id = Guid.NewGuid(), Seniority = Seniority.Senior, CurrentChatsCount = 0 }
            };

            _mockTeamManagementService.Setup(x => x.GetAvailableAgents()).Returns(agents);
            _mockMainQueue.Setup(x => x.Count).Returns(0);
            _mockOverflowQueue.Setup(x => x.Count).Returns(0);

            InvokePrivateMethod("AssignChatsFromQueue");

            _mockTeamManagementService.Verify(x => x.GetAvailableAgents(), Times.Once);
        }

        [Fact]
        public void CanAssignFromQueueTest()
        {
            var agents = new List<Agent>
            {
                new Agent { Id = Guid.NewGuid(), Name = "Test", Seniority = Seniority.Junior, CurrentChatsCount = 0 }
            };

            var session = new ChatSession { SessionId = Guid.NewGuid(), Status = ChatSessionStatus.Queued };

            var mockQueue = new Mock<IChatQueue>();
            mockQueue.Setup(x => x.Count).Returns(1);
            mockQueue.Setup(x => x.Peek()).Returns(session);
            mockQueue.Setup(x => x.Dequeue()).Returns(session);

            InvokeAssignFromQueuePrivateMethod(mockQueue.Object, agents);

            Assert.Equal(agents[0].Id, session.AssignedAgentId);
            Assert.Equal(ChatSessionStatus.Assigned, session.Status);
        }

        // Helper
        private void InvokePrivateMethod(string methodName)
        {
            var methodInfo = typeof(ChatAssignmentService).GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(_chatAssignmentService, null);
        }

        private void InvokeAssignFromQueuePrivateMethod(IChatQueue queue, List<Agent> availableAgents)
        {
            var methodInfo = typeof(ChatAssignmentService).GetMethod("AssignFromQueue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo.Invoke(_chatAssignmentService, new object[] { queue, availableAgents });
        }
    }
}
