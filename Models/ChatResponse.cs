namespace ChatSupportService.Models
{
    public class ChatResponse
    {
        public bool Success { get; set; }
        public Guid? SessionId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}