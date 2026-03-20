using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.Implementation
{
    public class OfficeHoursService : IOfficeHoursService
    {
        private readonly ITimeProviderService _timeProvider;

        public OfficeHoursService(ITimeProviderService timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public bool IsOfficeHours()
        { 
            var now = _timeProvider.GetCurrentTime();
            var hour = now.Hour;

            // Assuming office hours are from 8 AM to 6 PM, Monday to Friday
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
                return false;

            return hour >= 8 && hour < 18;
        }
    }
}