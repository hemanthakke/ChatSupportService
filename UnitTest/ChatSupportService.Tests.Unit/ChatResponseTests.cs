using ChatSupportService.Models;

namespace ChatSupportService.Tests.Unit
{
    public class ChatResponseTests
    {
        [Fact]
        public void CanChatResponseAllPropertiesTest()
        {
            var expectedSuccess = true;
            var expectedSessionId = Guid.NewGuid();
            var expectedStatus = "OK";
            var expectedMessage = "Chat Initiated";

            var chatResponse = new ChatResponse
            {
                Success = expectedSuccess,
                SessionId = expectedSessionId,
                Status = expectedStatus,
                Message = expectedMessage
            };

            Assert.Equal(expectedSuccess, chatResponse.Success);
            Assert.Equal(expectedSessionId, chatResponse.SessionId);
            Assert.Equal(expectedStatus, chatResponse.Status);  
            Assert.Equal(expectedMessage, chatResponse.Message);
        }
    }
}
