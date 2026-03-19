namespace ChatSupportService.Models
{
    public class Team
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Agent> Agents { get; set; } = new();
        public ShiftType ShiftType { get; set; }
        public bool IsOverflowTeam { get; set; }

        public int CalculateCapacity()
        {
            return (int)Agents.Sum(agent => 10 * agent.GetSeniorityMultiplier());
        }

        public int CalculateMaxQueuesLength()
        {
            return (int)(CalculateCapacity() * 1.5);
        }
    }
}