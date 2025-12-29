using System.Diagnostics.Tracing;

namespace Project_Dotnet_4_V2.Server.DTO.Dashboard
{
    public class DashboardStatsDTO
    {
        public int TotalEvents { get; set; }
        public int TotalReservations { get; set; }
        public int TotalSeatsReserved { get; set; }
        public int TotalSeatsPaid { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PendingRevenue { get; set; }
        public List<EventStatsDTO> EventsStats { get; set; } = new();
    }
}
