namespace ChatSupportService.Models
{
    public class Agent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Seniority Seniority { get; set; }
        public string TeamId { get; set; }
        public ShiftType CurrentShift { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public int CurrentChatsCount { get; set; }
        public bool IsAvailableForNewChats { get; set; }
        public int MaxConcurrency => (int)(10 * GetSeniorityMultiplier());

        public double GetSeniorityMultiplier()
        {
            return Seniority switch
            {
                Seniority.Junior => 0.4,
                Seniority.MidLevel => 0.6,
                Seniority.Senior => 0.8,
                Seniority.TeamLead => 0.5,
                _ => 0.4
            };
        }
    }
}