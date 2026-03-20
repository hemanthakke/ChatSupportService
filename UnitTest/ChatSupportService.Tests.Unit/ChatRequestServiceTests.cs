using ChatSupportService.Models;
using ChatSupportService.Services.Implementation;
using ChatSupportService.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatSupportService.Tests.Unit
{
    public class ChatRequestServiceTests
    {
        private readonly Mock<IChatQueue> _mainQueueMock;
        private readonly Mock<IChatQueue> _overflowQueueMock;
        private readonly Mock<ITeamManagementService> _teamManagementServiceMock;
        private readonly Mock<IOfficeHoursService> _officeHoursServiceMock;
        private readonly Mock<ILogger<ChatRequestService>> _loggerMock;
        private readonly ChatRequestService _chatRequestService;

        public ChatRequestServiceTests()
        {
            _mainQueueMock = new Mock<IChatQueue>();
            _overflowQueueMock = new Mock<IChatQueue>();
            _teamManagementServiceMock = new Mock<ITeamManagementService>();
            _officeHoursServiceMock = new Mock<IOfficeHoursService>();
            _loggerMock = new Mock<ILogger<ChatRequestService>>();

            _chatRequestService = new ChatRequestService(
                _mainQueueMock.Object,
                _overflowQueueMock.Object,
                _teamManagementServiceMock.Object,
                _officeHoursServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CanInitiateChatAsyncSuccessTest()
        {
            // Arrange
            var chatRequest = new ChatRequest { UserId = "hemant", Message = "Hello" };

            _teamManagementServiceMock.Setup(s => s.CalculateMaxQueueLength(false)).Returns(5);
            _mainQueueMock.Setup(q => q.IsFull(5)).Returns(false);
            _officeHoursServiceMock.Setup(s => s.IsOfficeHours()).Returns(true);

            // Act
            var response = await _chatRequestService.InitiateChatAsync(chatRequest);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("OK", response.Status);
        }

        [Fact]
        public async Task CanInitiateChatAsyncRefusedTest()
        {
            // Arrange
            var chatRequest = new ChatRequest { UserId = "hemant", Message = "Hello" };

            _teamManagementServiceMock.Setup(s => s.CalculateMaxQueueLength(false)).Returns(5);
            _mainQueueMock.Setup(q => q.IsFull(5)).Returns(true);
            _officeHoursServiceMock.Setup(s => s.IsOfficeHours()).Returns(false);

            // Act
            var response = await _chatRequestService.InitiateChatAsync(chatRequest);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("REFUSED", response.Status);
        }        

        [Fact]
        public async Task CanGetChatStatusAsyncTest()
        {
            // Arrange
            var nonExistSessionId = Guid.NewGuid();

            // Act
            var response = await _chatRequestService.GetChatStatusAsync(nonExistSessionId);

            // Assert
            Assert.False(response.Success);
            Assert.Equal("Chat session not found", response.Message);
        }

        [Fact]
        public async Task CanGetChatStatusAsyncQueuedTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                Status = ChatSessionStatus.Queued,
                CreatedAt = DateTime.Now,
                LastPolledAt = DateTime.Now.AddMinutes(-1),
                MissedPollCount = 5
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = await _chatRequestService.GetChatStatusAsync(sessionId);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Queued", response.Status);
        }

        [Fact]
        public async Task CanGetChatStatusAsyncAssignedTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var agentId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                AssignedAgentId = agentId,
                Status = ChatSessionStatus.Assigned,
                CreatedAt = DateTime.Now,
                LastPolledAt = DateTime.Now.AddMinutes(-1)
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = await _chatRequestService.GetChatStatusAsync(sessionId);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Assigned", response.Status);
        }

        [Fact]
        public async Task CanGetChatStatusAsyncActiveTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                Status = ChatSessionStatus.Active,
                CreatedAt = DateTime.Now,
                LastPolledAt = DateTime.Now.AddMinutes(-1)
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = await _chatRequestService.GetChatStatusAsync(sessionId);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Active", response.Status);
        }

        [Fact]
        public async Task CanGetChatStatusAsyncInactiveTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                Status = ChatSessionStatus.Inactive,
                CreatedAt = DateTime.Now,
                LastPolledAt = DateTime.Now.AddMinutes(-1)
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = await _chatRequestService.GetChatStatusAsync(sessionId);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Inactive", response.Status);
        }

        [Fact]
        public async Task CanGetChatStatusAsyncCompletedTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                Status = ChatSessionStatus.Completed,
                CreatedAt = DateTime.Now,
                LastPolledAt = DateTime.Now.AddMinutes(-1)
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = await _chatRequestService.GetChatStatusAsync(sessionId);

            // Assert
            Assert.True(response.Success);
            Assert.Equal("Completed", response.Status);
        }

        [Fact]
        public async Task CanGetSessionTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                Status = ChatSessionStatus.Queued,
                CreatedAt = DateTime.Now
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = _chatRequestService.GetSession(sessionId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(sessionId, response.SessionId);
        }

        [Fact]
        public async Task CanUpdateSessionTest()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            var session = new ChatSession
            {
                SessionId = sessionId,
                UserId = "hemant",
                Messsage = "Hello",
                Status = ChatSessionStatus.Queued
            };

            _chatRequestService.UpdateSession(session);

            // Act
            var response = _chatRequestService.GetSession(sessionId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(sessionId, response.SessionId);
        }
    }
}
