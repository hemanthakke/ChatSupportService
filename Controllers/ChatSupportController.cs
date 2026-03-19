using Microsoft.AspNetCore.Mvc;
using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class ChatSupportController : ControllerBase
    {        
        private readonly IChatRequestService _chatRequestService;
        private readonly ILogger<ChatSupportController> _loggerChatSupportController;

        public ChatSupportController(
            IChatRequestService chatRequestService,
            ILogger<ChatSupportController> loggerChatSupportController)
        {
            _chatRequestService = chatRequestService;
            _loggerChatSupportController = loggerChatSupportController;
        }

        [HttpPost("RequestChatSupport")]
        [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RequestChatSupport([FromBody] ChatRequest request)
        {
            _loggerChatSupportController.LogInformation("Received chat support request for user: {UserId}", request.UserId);

            var response = await _chatRequestService.InitiateChatAsync(request);

            if (response.Success)
            { 
                return Ok(response);
            }

            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        [HttpGet("GetChatStatus/{sessionId}")]
        [ProducesResponseType(typeof(ChatResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatStatus(Guid sessionId)
        {
            _loggerChatSupportController.LogInformation("Getting Chat Status for sessionId: {SessionId}", sessionId.ToString());

            var response = await _chatRequestService.GetChatStatusAsync(sessionId);

            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }
    }
}