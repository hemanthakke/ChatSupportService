using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.Implementation
{
    public class SystemTimeProviderService : ITimeProviderService
    {
        public DateTime GetCurrentTime() => DateTime.Now;
    }
}