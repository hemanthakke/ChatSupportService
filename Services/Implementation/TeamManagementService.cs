using ChatSupportService.Models;
using ChatSupportService.Services.Interfaces;

namespace ChatSupportService.Services.Implementation
{
    public class TeamManagementService : ITeamManagementService
    {
        private readonly List<Team> _teams;
        private readonly Team _overflowTeam;
        private readonly ITimeProviderService _timeProviderService;

        public TeamManagementService(ITimeProviderService timeProviderService = null)
        {
            // Team Initialization
            _timeProviderService = timeProviderService ?? new SystemTimeProviderService();
            _teams = InitializeTeams();
            _overflowTeam = InitializeOverflowTeam();            
        }

        private List<Team> InitializeTeams()
        {
            return new List<Team>
            {
                new Team
                {
                    Id = "TeamA",
                    Name = "Team A",
                    ShiftType = ShiftType.Day,
                    Agents = new List<Agent>
                    {
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent A1", Seniority = Seniority.TeamLead, TeamId = "TeamA" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent A2", Seniority = Seniority.MidLevel, TeamId = "TeamA" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent A3", Seniority = Seniority.MidLevel, TeamId = "TeamA" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent A4", Seniority = Seniority.Junior, TeamId = "TeamA" }
                    }
                },
                new Team
                {
                    Id = "TeamB",
                    Name = "Team B",
                    ShiftType = ShiftType.Day,
                    Agents = new List<Agent>
                    {
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent B1", Seniority = Seniority.Senior, TeamId = "TeamB" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent B2", Seniority = Seniority.MidLevel, TeamId = "TeamB" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent B3", Seniority = Seniority.Junior, TeamId = "TeamB" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent B4", Seniority = Seniority.Junior, TeamId = "TeamB" }
                    }
                },
                new Team
                {
                    Id = "TeamC",
                    Name = "Team C (Night Shift)",
                    ShiftType = ShiftType.Evening,
                    Agents = new List<Agent>
                    {
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent C1", Seniority = Seniority.MidLevel, TeamId = "TeamC" },
                        new Agent{  Id = Guid.NewGuid(), Name = "Agent C2", Seniority = Seniority.MidLevel, TeamId = "TeamC" }
                    }
                }
            };
        }

        private Team InitializeOverflowTeam()
        {
            var overflowAgents = new List<Agent>();

            for (int cnt = 1; cnt <= 6; cnt++)
            {
                overflowAgents.Add(new Agent
                {
                    Id = Guid.NewGuid(),
                    Name = $"Overflow Agent {cnt}",
                    Seniority = Seniority.Junior,
                    TeamId = "OverflowTeam"
                });
            }

            return new Team
            {
                Id = "OverflowTeam",
                Name = "Overflow Team",
                Agents = overflowAgents,
                IsOverflowTeam = true
            };
        }

        public List<Team> GetActiveTeams()
        {
            var currentHour = _timeProviderService.GetCurrentTime().Hour;

            var activeShift = currentHour switch
            {
                >= 0 and < 8 => ShiftType.Morning,
                >= 8 and < 16 => ShiftType.Day,
                _ => ShiftType.Evening
            };

            return _teams.Where(team => team.ShiftType == activeShift).ToList();
        }

        public Team GetOverflowTeam() => _overflowTeam;

        public List<Agent> GetAvailableAgents()
        {
            var activeTeams = GetActiveTeams();

            var agents = activeTeams
                .SelectMany(team => team.Agents)
                .Where(agent => agent.IsAvailableForNewChats && agent.CurrentChatsCount < agent.MaxConcurrency && DateTime.Now < agent.ShiftEndTime)
                .OrderBy(agent => agent.Seniority) // Junior agents get assigned first
                .ToList();

            return agents;
        }

        public int CalculateTotalCapacity(bool includeOverflow)
        {
            var capacity = GetActiveTeams().Sum(team => team.CalculateCapacity());

            if (includeOverflow)
            {
                capacity += _overflowTeam.CalculateCapacity();
            }

            return capacity;
        }

        public int CalculateMaxQueueLength(bool includeOverflow)
        {
            var capacity = CalculateTotalCapacity(includeOverflow);
            return (int)(capacity * 1.5); // Assuming max queue length is 1.5 times the total capacity
        }
    }
}