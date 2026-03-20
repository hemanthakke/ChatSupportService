using ChatSupportService.Services.Implementation;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Tests.Unit
{
    // Fake time provider
    public class FakeTimeProviderService : ITimeProviderService
    {
        private readonly DateTime _fixedTime;

        public FakeTimeProviderService(DateTime fixedTime)
        { 
            _fixedTime = fixedTime;
        }

        public DateTime GetCurrentTime() => _fixedTime;
    }

    public class OfficeHoursServiceTests
    {
        // Helper method
        private OfficeHoursService CreateServiceWithTime(DateTime dateTime)
        {
            return new OfficeHoursService(new FakeTimeProviderService(dateTime));
        }

        private DateTime CreateDateTime(int year, int month, int day, int hour, int minute = 0)
        {
            return new DateTime(year, month, day, hour, minute, 0);
        }

        [Fact]
        public void CanIsOfficeHoursMon8AMTest()
        {
            var testTime = CreateDateTime(2025, 1, 6, 8, 0);
            var service = CreateServiceWithTime(testTime);

            var result = service.IsOfficeHours();

            Assert.True(result);
        }

        [Fact]
        public void CanIsOfficeHoursSun2PMTest()
        {
            var testTime = CreateDateTime(2025, 1, 12, 14, 0);
            var service = CreateServiceWithTime(testTime);

            var result = service.IsOfficeHours();

            Assert.False(result);
        }
    }
}
