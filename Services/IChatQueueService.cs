using ChatSupportService.Models;

namespace ChatSupportService.Services
{
    public interface IChatQueueService
    {
        bool TryEnqueue(ChatSession session);
        int Capacity { get; }
        int Count { get; }
    }
}