using ChatSupportService.Models;

namespace ChatSupportService.Services.Interfaces
{
    public interface ITeamManagementService
    {
        List<Team> GetActiveTeams();
        Team GetOverflowTeam();
        List<Agent> GetAvailableAgents();
        int CalculateTotalCapacity(bool includeOverflow);
        int CalculateMaxQueueLength(bool includeOverflow);
    }
}
