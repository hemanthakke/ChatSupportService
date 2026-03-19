namespace ChatSupportService.Models
{
    public enum Seniority
    {
        Junior,
        MidLevel,
        Senior,
        TeamLead
    }

    public enum ChatSessionStatus 
    { 
        Queued,
        Assigned,
        Active,
        Inactive,
        Completed,
        Refused
    }

    public enum ShiftType
    {
        Morning,  // 00:00 - 08:00
        Day,      // 08:00 - 16:00
        Evening   // 16:00 - 24:00
    }
}