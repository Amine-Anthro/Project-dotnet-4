namespace Project_Dotnet_4_V2.Server.DTO.Dashboard
{
    public class EventStatsDTO
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int ReservedSeats { get; set; }
        public int PaidSeats { get; set; }
        public decimal FillRate { get; set; } 
        public decimal Revenue { get; set; }
        public decimal PotentialRevenue { get; set; }
    }
}

