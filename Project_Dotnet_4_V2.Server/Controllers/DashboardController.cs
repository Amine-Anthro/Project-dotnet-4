using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Dotnet_4_V2.Server.Services;

namespace Project_Dotnet_4_V2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController : Controller
    {
        
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Obtenir les statistiques globales
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Obtenir les statistiques d'un événement spécifique
        /// </summary>
        [HttpGet("events/{eventId}/stats")]
        public async Task<IActionResult> GetEventStats(int eventId)
        {
            var stats = await _dashboardService.GetEventStatsAsync(eventId);
            return Ok(stats);
        }
    }
}
