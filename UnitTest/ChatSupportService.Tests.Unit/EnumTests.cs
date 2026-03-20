using ChatSupportService.Models;

namespace ChatSupportService.Tests.Unit
{
    public class EnumTests
    {
        [Fact]
        public void CanSeniorityEnumValuesTest()
        {
            Assert.Equal(0, (int)Seniority.Junior);
            Assert.Equal(1, (int)Seniority.MidLevel);
            Assert.Equal(2, (int)Seniority.Senior);
            Assert.Equal(3, (int)Seniority.TeamLead);
        }

        [Fact]
        public void CanChatSessionStatusEnumValuesTest()
        {
            Assert.Equal(0, (int)ChatSessionStatus.Queued);
            Assert.Equal(1, (int)ChatSessionStatus.Assigned);
            Assert.Equal(2, (int)ChatSessionStatus.Active);
            Assert.Equal(3, (int)ChatSessionStatus.Inactive);
            Assert.Equal(4, (int)ChatSessionStatus.Completed);
            Assert.Equal(5, (int)ChatSessionStatus.Refused);
        }

        [Fact]
        public void CanShiftTypeEnumValuesTest()
        {
            Assert.Equal(0, (int)ShiftType.Morning);
            Assert.Equal(1, (int)ShiftType.Day);
            Assert.Equal(2, (int)ShiftType.Evening);
        }
    }
}