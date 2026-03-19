using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.Implementation
{
    public class InMemoryChatQueue : IChatQueue
    {
        private readonly Queue<ChatSession> _queue = new();
        private readonly object _lock = new();

        public void Enqueue(ChatSession session)
        {
            lock (_lock)
            {
                _queue.Enqueue(session);
            }
        }

        public ChatSession Dequeue()
        {
            lock (_lock)
            {
                return _queue.Count > 0 ? _queue.Dequeue() : null;
            }
        }

        public ChatSession Peek()
        {
            lock (_lock)
            {
                return _queue.Count > 0 ? _queue.Peek() : null;
            }
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

        public bool IsFull(int maxLength)
        {
            lock (_lock)
            {
                return _queue.Count >= maxLength;
            }
        }

        public List<ChatSession> GetAll()
        { 
            lock (_lock)
            {
                return _queue.ToList();
            }
        }

        public void Remove(Guid sessionId)
        {
            lock (_lock)
            {
                var tempQueue = new Queue<ChatSession>();
                while (_queue.Count > 0)
                {
                    var session = _queue.Dequeue();
                    if (session.SessionId != sessionId)
                    {
                        tempQueue.Enqueue(session);
                    }
                }
                while (tempQueue.Count > 0)
                {
                    _queue.Enqueue(tempQueue.Dequeue());
                }
            }
        }
    }
}