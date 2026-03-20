using ChatSupportService.Models;

namespace ChatSupportService.Tests.Unit
{
    public class ChatRequestTests
    {
        [Fact]
        public void CanChatRequestAllPropertiesTest()
        {
            var expectedUserId = "hemant";
            var expectedMessage = "Hi, this is Hemant";

            var chatRequest = new ChatRequest
            {
                UserId = expectedUserId,
                Message = expectedMessage
            };

            Assert.Equal(expectedUserId, chatRequest.UserId);
            Assert.Equal(expectedMessage, chatRequest.Message);
        }
    }
}
