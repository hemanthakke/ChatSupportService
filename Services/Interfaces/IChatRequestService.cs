using ChatSupportService.Models;

namespace ChatSupportService.Services.Interfaces
{
    public interface IChatRequestService
    {
        Task<ChatResponse> InitiateChatAsync(ChatRequest request);
        Task<ChatResponse> GetChatStatusAsync(Guid sessionId);
        void UpdateSession(ChatSession session);
    }
}