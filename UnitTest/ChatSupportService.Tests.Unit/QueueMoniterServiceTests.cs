using ChatSupportService.Models;
using ChatSupportService.Services.BackgroundServices;
using ChatSupportService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatSupportService.Tests.Unit
{
    public class QueueMoniterServiceTests
    {
        private readonly Mock<IChatQueue> _mockMainQueue;
        private readonly Mock<IChatQueue> _mockOverflowQueue;
        private readonly Mock<IChatRequestService> _mockChatRequestService;
        private readonly Mock<ILogger<QueueMoniterService>> _mockLogger;
        private readonly QueueMoniterService _queueMoniterService;

        public QueueMoniterServiceTests()
        {
            _mockMainQueue = new Mock<IChatQueue>();
            _mockOverflowQueue = new Mock<IChatQueue>();
            _mockChatRequestService = new Mock<IChatRequestService>();
            _mockLogger = new Mock<ILogger<QueueMoniterService>>();

            _queueMoniterService = new QueueMoniterService(
                    _mockMainQueue.Object,
                    _mockOverflowQueue.Object,
                    _mockChatRequestService.Object,
                    _mockLogger.Object
            );
        }

        [Fact]
        public async Task CanExecuteAsyncTest()
        {
            var mainQueueSessions = new List<ChatSession>();
            var overflowQueueSessions = new List<ChatSession>();

            _mockMainQueue.Setup(x => x.GetAll()).Returns(mainQueueSessions);
            _mockOverflowQueue.Setup(x => x.GetAll()).Returns(overflowQueueSessions);

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(500));

            await _queueMoniterService.StartAsync(cts.Token);
            await Task.Delay(1500);
            await _queueMoniterService.StopAsync(CancellationToken.None);

            _mockMainQueue.Verify(x => x.GetAll(), Times.AtLeastOnce);
            _mockOverflowQueue.Verify(x => x.GetAll(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CanExecuteAsyncExceptionTest()
        {
            var cts = new CancellationTokenSource();
            var expectedException = new InvalidOperationException("Test exception");
            var exceptionThrown = false;

            _mockMainQueue.Setup(q => q.GetAll())
                .Returns(() =>
                {
                    if (!exceptionThrown)
                    { 
                        exceptionThrown = true;
                        throw expectedException;
                    }
                    return new List<ChatSession>();
                });

            _mockOverflowQueue.Setup(q => q.GetAll()).Returns(new List<ChatSession>());

            cts.CancelAfter(TimeSpan.FromMilliseconds(2500));

            await _queueMoniterService.StartAsync(cts.Token);
            await Task.Delay(2600);
            await _queueMoniterService.StopAsync(CancellationToken.None);
        }

        [Fact]
        public void CanMonitorQueueTest()
        { 
            var session = new ChatSession
            {
                SessionId = Guid.NewGuid(),
                LastPolledAt = DateTime.Now.AddSeconds(-2),
                Status = ChatSessionStatus.Active,
                MissedPollCount = 2
            };

            _mockMainQueue.Setup(q => q.GetAll()).Returns(new List<ChatSession> { session });

            InvokeMoniterQueue(_mockMainQueue.Object);

            Assert.Equal(2, session.MissedPollCount);
            Assert.Equal(ChatSessionStatus.Active, session.Status);
        }

        private void InvokeMoniterQueue(IChatQueue queue)
        {
            var methodInfo = typeof(QueueMoniterService).GetMethod("MoniterQueue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo?.Invoke(_queueMoniterService, new object[] { queue });
        }
    }
}