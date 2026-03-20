using ChatSupportService.Models;

namespace ChatSupportService.Tests.Unit
{
    public class AgentTests
    {
        [Fact]
        public void CanGetSeniorityMultiplierJuniorTest()
        {
            var agent = new Agent { Seniority = Seniority.Junior };

            var result = agent.GetSeniorityMultiplier();

            Assert.Equal(0.4, result);
        }

        [Fact]
        public void CanGetSeniorityMultiplierMidLevelTest()
        {
            var agent = new Agent { Seniority = Seniority.MidLevel };

            var result = agent.GetSeniorityMultiplier();

            Assert.Equal(0.6, result);
        }

        [Fact]
        public void CanGetSeniorityMultiplierTeamLeadTest()
        {
            var agent = new Agent { Seniority = Seniority.TeamLead };

            var result = agent.GetSeniorityMultiplier();

            Assert.Equal(0.5, result);
        }

        [Fact]
        public void CanGetSeniorityMultiplierInvalidTest()
        {
            var agent = new Agent { Seniority = (Seniority)999 };

            var result = agent.GetSeniorityMultiplier();

            Assert.Equal(0.4, result);
        }

        [Fact]
        public void CanMaxConcurrenyTest()
        {
            var agent = new Agent { Seniority = Seniority.Junior };

            var result = agent.MaxConcurrency;

            Assert.Equal(4, result);
        }

        [Fact]
        public void CanAgentAllPropertiesTest()
        {
            var expectedId = Guid.NewGuid();
            var expectedName = "John Doe";
            var expectedSeniority = Seniority.MidLevel;
            var expectedTeamId = "TeamA";
            var expectedCurrentShift = ShiftType.Morning;
            var expectedShiftEndTime = DateTime.Now.AddHours(8);
            var expectedCurrentChatsCount = 3;
            var expectedIsAvailableForNewChats = true;

            var agent = new Agent 
            {  
                Id = expectedId,
                Name = expectedName,
                Seniority = expectedSeniority,
                TeamId = expectedTeamId,
                CurrentShift = expectedCurrentShift,
                ShiftEndTime = expectedShiftEndTime,
                CurrentChatsCount = expectedCurrentChatsCount,  
                IsAvailableForNewChats = expectedIsAvailableForNewChats
            };

            Assert.Equal(expectedId, agent.Id);
            Assert.Equal(expectedName, agent.Name);
            Assert.Equal(expectedSeniority, agent.Seniority);
            Assert.Equal(expectedTeamId, agent.TeamId);
            Assert.Equal(expectedCurrentShift, agent.CurrentShift);
            Assert.Equal(expectedShiftEndTime, agent.ShiftEndTime);
            Assert.Equal(expectedCurrentChatsCount, agent.CurrentChatsCount);
            Assert.Equal(expectedIsAvailableForNewChats, agent.IsAvailableForNewChats);
        }
    }
}