using ChatSupportService.Models;

namespace ChatSupportService.Services.Interfaces
{
    public interface IChatQueue
    {
        void Enqueue(ChatSession session);
        ChatSession Dequeue();
        ChatSession Peek();
        int Count { get; }
        bool IsFull(int maxLength);
        List<ChatSession> GetAll();
        void Remove(Guid sessionId);
    }
}
