using ChatSupportService.Models;
using ChatSupportService.Services.BackgroundServices;
using ChatSupportService.Services.Implementation;
using ChatSupportService.Services.Interfaces;
using Moq;

namespace ChatSupportService.Tests.Unit
{
    public class TeamManagementServiceTests
    {
        private readonly TeamManagementService _teamManagementService;

        public TeamManagementServiceTests()
        {
            _teamManagementService = new TeamManagementService();
        }

        private List<Team> GetInitializedTeams()
        {
            var methodInfo = typeof(TeamManagementService).GetMethod(
                "InitializeTeams", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                );

            return (List<Team>)methodInfo.Invoke(_teamManagementService, null);
        }

        private Mock<ITimeProviderService> CreateTimeProviderServiceMock(DateTime time)
        { 
            var mock = new Mock<ITimeProviderService>();
            mock.Setup(x => x.GetCurrentTime()).Returns(time);
            return mock;
        }

        private void ConfigureAgent(Agent agent, bool isAvailable = true, int currentChats = 0, int maxConcurrency = 10, DateTime? shiftEndTime = null)
        { 
            agent.IsAvailableForNewChats = isAvailable; ;
            agent.CurrentChatsCount = currentChats;
            agent.ShiftEndTime = shiftEndTime ?? DateTime.Now.AddHours(2);
        }

        private List<Agent> GetAllAgentsFromService()
        { 
            return _teamManagementService
                .GetActiveTeams()
                .SelectMany(team => team.Agents)
                .ToList();
        }

        [Fact]
        public void CanInitializeTeamsTest()
        {
            var teams = GetInitializedTeams();

            Assert.NotNull(teams);
            Assert.Equal(3, teams.Count);
        }

        [Fact]
        public void CanInitializeOverflowTeamTest()
        {
            var overflowTeam = _teamManagementService.GetOverflowTeam();

            Assert.NotNull(overflowTeam);
            Assert.Equal("OverflowTeam", overflowTeam.Id);
        }

        [Fact]
        public void CanGetActiveTeamsMorningShiftTest()
        {
            var timeProviderService = CreateTimeProviderServiceMock(new DateTime(2025, 1, 1, 5, 0, 0, 0));
            var service = new TeamManagementService(timeProviderService.Object);

            var activeTeams = service.GetActiveTeams();

            Assert.NotNull(activeTeams);
            Assert.All(activeTeams, team => Assert.Equal(ShiftType.Morning, team.ShiftType));
        }

        [Fact]
        public void CanGetActiveTeamsDayShiftTest()
        {
            var timeProviderService = CreateTimeProviderServiceMock(new DateTime(2025, 1, 1, 12, 0, 0, 0));
            var service = new TeamManagementService(timeProviderService.Object);

            var activeTeams = service.GetActiveTeams();

            Assert.NotNull(activeTeams);
            Assert.All(activeTeams, team => Assert.Equal(ShiftType.Day, team.ShiftType));
        }

        [Fact]
        public void CanGetActiveTeamsEveningShiftTest()
        {
            var timeProviderService = CreateTimeProviderServiceMock(new DateTime(2025, 1, 1, 20, 0, 0, 0));
            var service = new TeamManagementService(timeProviderService.Object);

            var activeTeams = service.GetActiveTeams();

            Assert.NotNull(activeTeams);
            Assert.All(activeTeams, team => Assert.Equal(ShiftType.Evening, team.ShiftType));
        }

        [Fact]
        public void CanGetAvailableAgentsTest()
        {
            var agents = GetAllAgentsFromService();

            foreach (var agent in agents)
            {
                ConfigureAgent(agent, isAvailable: true, currentChats: 0, maxConcurrency: 10, shiftEndTime: DateTime.Now.AddHours(3));
            }

            var result = _teamManagementService.GetAvailableAgents();

            Assert.NotNull(result);
        }

        [Fact]
        public void CanCalculateTotalCapacityTest()
        {
            var capacity = _teamManagementService.CalculateTotalCapacity(includeOverflow: false);
            var capacityOverflow = _teamManagementService.CalculateTotalCapacity(includeOverflow: true);

            Assert.True(capacityOverflow > capacity);

            var overflowTeam = _teamManagementService.GetOverflowTeam();
            var excpectedDiff = overflowTeam.CalculateCapacity();
            Assert.Equal(excpectedDiff, capacityOverflow - capacity);
        }

        [Fact]
        public void CanCalculateMaxQueueLengthTest()
        {
            var maxQueueLength = _teamManagementService.CalculateMaxQueueLength(includeOverflow: false);

            Assert.True(maxQueueLength > 0);
        }
    }
}
