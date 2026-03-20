using ChatSupportService.Controllers;
using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatSupportService.Tests.Unit
{
    public class ChatSupportControllerTests
    {
        private readonly Mock<IChatRequestService> _mockChatRequestService;
        private readonly Mock<ILogger<ChatSupportController>> _mockLogger;
        private readonly ChatSupportController _controller;

        public ChatSupportControllerTests()
        {
            _mockChatRequestService = new Mock<IChatRequestService>();
            _mockLogger = new Mock<ILogger<ChatSupportController>>();
            _controller = new ChatSupportController(_mockChatRequestService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CanRequestChatSupportOkResultTest()
        {
            var chatRequest = new ChatRequest { UserId = "user123" };
            var chatResponse = new ChatResponse { Success = true, SessionId = Guid.NewGuid() };

            _mockChatRequestService
                .Setup(service => service.InitiateChatAsync(chatRequest))
                .ReturnsAsync(chatResponse);

            var result = await _controller.RequestChatSupport(chatRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ChatResponse>(okResult.Value);
            Assert.True(returnedResponse.Success);
        }

        [Fact]
        public async Task CanRequestChatSupportServiceUnavailableTest()
        {
            // Arrange
            var chatRequest = new ChatRequest { UserId = "user123" };
            var chatResponse = new ChatResponse { Success = false, SessionId = Guid.Empty };

            _mockChatRequestService
                .Setup(service => service.InitiateChatAsync(chatRequest))
                .ReturnsAsync(chatResponse);

            var result = await _controller.RequestChatSupport(chatRequest);

            var serviceUnavailableResult = Assert.IsType<ObjectResult>(result);
            Assert.IsType<ChatResponse>(serviceUnavailableResult.Value);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, serviceUnavailableResult.StatusCode);
            var returnedResponse = Assert.IsType<ChatResponse>(serviceUnavailableResult.Value);
            Assert.False(returnedResponse.Success);
        }

        [Fact]
        public async Task CanGetChatStatusOkResultTest()
        {
            var sessionId = Guid.NewGuid();
            var chatResponse = new ChatResponse { Success = true, SessionId = sessionId };

            _mockChatRequestService
                .Setup(service => service.GetChatStatusAsync(sessionId))
                .ReturnsAsync(chatResponse);

            var result = await _controller.GetChatStatus(sessionId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedResponse = Assert.IsType<ChatResponse>(okResult.Value);
            Assert.True(returnedResponse.Success);
        }

        [Fact]
        public async Task CanGetChatStatusNotFoundTest()
        {
            var sessionId = Guid.NewGuid();
            var chatResponse = new ChatResponse { Success = false, SessionId = sessionId };

            _mockChatRequestService
                .Setup(service => service.GetChatStatusAsync(sessionId))
                .ReturnsAsync(chatResponse);

            var result = await _controller.GetChatStatus(sessionId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            var returnedResponse = Assert.IsType<ChatResponse>(notFoundResult.Value);
            Assert.False(returnedResponse.Success);
        }
    }
}