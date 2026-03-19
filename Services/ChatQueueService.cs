using ChatSupportService.Models;

namespace ChatSupportService.Services
{
    public class ChatQueueService : IChatQueueService
    {
        private readonly Queue<ChatSession> _queue = new();
        private readonly object _lock = new();
        public int Capacity { get; }

        public ChatQueueService()
        {
            // Default capacity; could be configured later
            Capacity = 5;
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }

        public bool TryEnqueue(ChatSession session)
        {
            lock (_lock)
            {
                if (_queue.Count >= Capacity)
                    return false;

                _queue.Enqueue(session);
                return true;
            }
        }
    }
}