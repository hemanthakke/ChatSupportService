using ChatSupportService.Models;

namespace ChatSupportService.Tests.Unit
{
    public class ChatSessionTests
    {
        [Fact]
        public void CanAgentAllPropertiesTest()
        {
            var expectedSessionId = Guid.NewGuid();
            var expectedUserId = "hemant";
            var expectedStatus = ChatSessionStatus.Active;
            var expectedCreatedAt = DateTime.UtcNow.AddMinutes(-10);
            var expectedLastPolledAt = DateTime.UtcNow;
            var expectedAgentId = Guid.NewGuid();
            var expectedMissedPollCount = 3;
            var expectedMessage = "Test Message";

            var chatSession = new ChatSession
            {
                SessionId = expectedSessionId,
                UserId = expectedUserId,
                Status = expectedStatus,
                CreatedAt = expectedCreatedAt,
                LastPolledAt = expectedLastPolledAt,
                AssignedAgentId = expectedAgentId,
                MissedPollCount = expectedMissedPollCount,
                Messsage = expectedMessage
            };

            Assert.Equal(expectedSessionId, chatSession.SessionId);
            Assert.Equal(expectedUserId, chatSession.UserId);
            Assert.Equal(expectedStatus, chatSession.Status);
            Assert.Equal(expectedCreatedAt, chatSession.CreatedAt);
            Assert.Equal(expectedLastPolledAt, chatSession.LastPolledAt);
            Assert.Equal(expectedAgentId, chatSession.AssignedAgentId);
            Assert.Equal(expectedMissedPollCount, chatSession.MissedPollCount);
            Assert.Equal(expectedMessage, chatSession.Messsage);
        }
    }
}
