using ChatSupportService.Models;
using ChatSupportService.Services.Implementation;
using NuGet.Frameworks;

namespace ChatSupportService.Tests.Unit
{
    public class InMemoryChatQueueTests
    {
        private ChatSession CreateChatSession(string userName = "TestClient")
        {
            return new ChatSession
            { 
                SessionId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UserId = userName,
                Messsage = "Hello, I need help with my order.",
            };
        }

        [Fact]
        public void CanEnqueueTest()
        { 
            var queue = new InMemoryChatQueue();
            var session = CreateChatSession();

            queue.Enqueue(session);

            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void CanDequeueTest()
        {
            var queue = new InMemoryChatQueue();
            var session1 = CreateChatSession("User1");
            var session2 = CreateChatSession("User2");

            queue.Enqueue(session1);
            queue.Enqueue(session2);

            var res = queue.Dequeue();

            Assert.NotNull(res);
            Assert.Equal(session1.SessionId, res.SessionId);
        }

        [Fact]
        public void CanPeekTest()
        {
            var queue = new InMemoryChatQueue();
            var session1 = CreateChatSession("User1");
            var session2 = CreateChatSession("User2");

            queue.Enqueue(session1);
            queue.Enqueue(session2);

            var res = queue.Peek();

            Assert.NotNull(res);
            Assert.Equal(session1.SessionId, res.SessionId);
        }

        [Fact]
        public void CanCountTest()
        {
            var queue = new InMemoryChatQueue();
            var session1 = CreateChatSession("User1");
            var session2 = CreateChatSession("User2");

            queue.Enqueue(session1);
            queue.Enqueue(session2);

            var res = queue.Dequeue();

            Assert.Equal(1, queue.Count);
        }

        [Fact]
        public void CanIsFullTest()
        {
            var queue = new InMemoryChatQueue();
            var session1 = CreateChatSession("User1");
            var session2 = CreateChatSession("User2");

            var isFull = queue.IsFull(5);

            Assert.False(isFull);
        }

        [Fact]
        public void CanGetAllTest()
        {
            var queue = new InMemoryChatQueue();
            var session1 = CreateChatSession("User1");

            var res1 = queue.GetAll();
            var res2 = queue.GetAll();

            Assert.NotSame(res1, res2);
        }

        [Fact]
        public void CanRemoveTest()
        {
            var queue = new InMemoryChatQueue();
            var session1 = CreateChatSession("User1");
            var session2 = CreateChatSession("User2");
            var session3 = CreateChatSession("User3");

            queue.Enqueue(session1);
            queue.Enqueue(session2);
            queue.Enqueue(session3);

            queue.Remove(session2.SessionId);

            Assert.Equal(2, queue.Count);
        }
    }
}
