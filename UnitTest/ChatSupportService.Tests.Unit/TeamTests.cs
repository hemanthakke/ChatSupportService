using ChatSupportService.Models;

namespace ChatSupportService.Tests.Unit
{
    public class TeamTests
    {
        [Fact]
        public void CanTeamAllPropertiesTest()
        {
            var expectedTeamId = "TeamA";
            var expectedTeamName = "Team A";

            var expectedId = Guid.NewGuid();
            var expectedName = "Hemant";
            var expectedSeniority = Seniority.MidLevel;
            var expectedTeam = "Team A";
            var expectedCurrentShift = ShiftType.Morning;
            var expectedShiftEndTime = DateTime.Now.AddHours(8);
            var expectedCurrentChatsCount = 3;
            var expectedIsAvailableForNewChats = true;

            var expectedAgent = new Agent
            {
                Id = expectedId,
                Name = expectedName,
                Seniority = expectedSeniority,
                TeamId = expectedTeam,
                CurrentShift = expectedCurrentShift,
                ShiftEndTime = expectedShiftEndTime,
                CurrentChatsCount = expectedCurrentChatsCount,
                IsAvailableForNewChats = expectedIsAvailableForNewChats
            };

            var expectedAgents = new List<Agent> { expectedAgent };

            var team = new Team
            {
                Id = expectedTeamId,
                Name = expectedTeamName,
                Agents = expectedAgents,
                ShiftType = expectedCurrentShift,
                IsOverflowTeam = false
            };

            Assert.Equal(expectedTeamId, team.Id);
            Assert.Equal(expectedTeamName, team.Name);
            Assert.Equal(expectedCurrentShift, team.ShiftType);
            Assert.False(team.IsOverflowTeam);
            Assert.NotNull(team.Agents);
            Assert.Single(team.Agents);
            Assert.Equal(expectedId, team.Agents[0].Id);
        }

        [Fact]
        public void CanCalculateCapacityTest()
        {
            var team = new Team
            {
                Id = "TeamA",
                Name = "Team A",
                ShiftType = ShiftType.Morning,
                IsOverflowTeam = false,
                Agents = new List<Agent>
                {
                    new Agent { Seniority = Seniority.Junior },
                    new Agent { Seniority = Seniority.MidLevel },
                    new Agent { Seniority = Seniority.TeamLead }
                }
            };
            var capacity = team.CalculateCapacity();

            Assert.Equal(15.0, capacity);
        }

        [Fact]
        public void CanCalculateMaxQueueLengthTest()
        {
            var team = new Team
            {
                Id = "TeamA",
                Name = "Team A",
                ShiftType = ShiftType.Morning,
                IsOverflowTeam = false,
                Agents = new List<Agent>
                {
                    new Agent { Seniority = Seniority.Junior },
                    new Agent { Seniority = Seniority.MidLevel },
                    new Agent { Seniority = Seniority.TeamLead }
                }
            };
            var maxQueueLength = team.CalculateMaxQueuesLength();
            Assert.Equal(22, maxQueueLength);
        }
    }
}