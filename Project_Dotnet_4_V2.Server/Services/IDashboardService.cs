using Project_Dotnet_4_V2.Server.DTO.Dashboard;
using System.Diagnostics.Tracing;

namespace Project_Dotnet_4_V2.Server.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDTO> GetDashboardStatsAsync(int? eventId = null);
        Task<EventStatsDTO> GetEventStatsAsync(int eventId);
    }
}
