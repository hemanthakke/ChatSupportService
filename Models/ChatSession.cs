namespace ChatSupportService.Models
{
    public class ChatSession
    {
        public Guid SessionId { get; set; }
        public string UserId { get; set; }
        public ChatSessionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastPolledAt { get; set; }
        public Guid? AssignedAgentId { get; set; }
        public int MissedPollCount { get; set; }
        public string Messsage { get; set; }
    }
}