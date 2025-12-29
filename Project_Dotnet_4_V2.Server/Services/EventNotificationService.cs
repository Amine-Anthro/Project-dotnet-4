using Microsoft.AspNetCore.SignalR;
using Project_Dotnet_4_V2.Server.Hubs;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class EventNotificationService : IEventNotificationService
    {
        private readonly IHubContext<EventHub> _hubContext;
        private readonly IServiceProvider _serviceProvider;
        public EventNotificationService(IHubContext<EventHub> hubContext, IServiceProvider serviceProvider)
        {
            _hubContext = hubContext;
            _serviceProvider = serviceProvider;
        }

        public async Task NotifySeatReservedAsync(int eventId, int seatId, string row, string number)
        {
            Console.WriteLine($"[Notification] Seat reserved: Event {eventId}, Seat {row}{number}");
            await _hubContext.Clients.Group($"event-{eventId}").SendAsync("SeatReserved", new
            {
                seatId,
                row,
                number,
                status = "Reserved"
            });

            await NotifyEventStatsUpdateAsync(eventId);
        }

        public async Task NotifySeatReleasedAsync(int eventId, int seatId, string row, string number)
        {
            Console.WriteLine($"[Notification] Seat released: Event {eventId}, Seat {row}{number}");

            await _hubContext.Clients.Group($"event-{eventId}").SendAsync("SeatReleased", new
            {
                seatId,
                row,
                number,
                status = "Available"
            });
            await NotifyEventStatsUpdateAsync(eventId);
        }

        public async Task NotifySeatPaidAsync(int eventId, int seatId, string row, string number)
        {
            Console.WriteLine($"[Notification] Seat paid: Event {eventId}, Seat {row}{number}");

            await _hubContext.Clients.Group($"event-{eventId}").SendAsync("SeatPaid", new
            {
                seatId,
                row,
                number,
                status = "Paid"
            });
            await NotifyEventStatsUpdateAsync(eventId);
        }

        public async Task NotifyDashboardUpdateAsync()
        {
            try
            {
                Console.WriteLine("[Dashboard] Starting NotifyDashboardUpdateAsync");

                using var scope = _serviceProvider.CreateScope();
                var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();

                Console.WriteLine("[Dashboard] DashboardService retrieved, getting stats...");
                var stats = await dashboardService.GetDashboardStatsAsync();

                Console.WriteLine($"[Dashboard] Stats retrieved: {stats.TotalEvents} events, Revenue: {stats.TotalRevenue}€");
                Console.WriteLine("[Dashboard] Sending DashboardUpdated to 'dashboard' group");

                await _hubContext.Clients.Group("dashboard").SendAsync("DashboardUpdated", stats);

                Console.WriteLine("[Dashboard] DashboardUpdated sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dashboard] ERROR in NotifyDashboardUpdateAsync: {ex.Message}");
                Console.WriteLine($"[Dashboard] StackTrace: {ex.StackTrace}");
            }
        }

        public async Task NotifyEventStatsUpdateAsync(int eventId)
        {
            try
            {
                Console.WriteLine($"[Dashboard] Starting NotifyEventStatsUpdateAsync for event {eventId}");

                using var scope = _serviceProvider.CreateScope();
                var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();

                Console.WriteLine($"[Dashboard] DashboardService retrieved, getting event stats...");
                var eventStats = await dashboardService.GetEventStatsAsync(eventId);

                Console.WriteLine($"[Dashboard] Event stats retrieved: {eventStats.EventName}, Fill rate: {eventStats.FillRate}%, Revenue: {eventStats.Revenue}€");
                Console.WriteLine($"[Dashboard] Sending EventStatsUpdated to 'dashboard' group");

                await _hubContext.Clients.Group("dashboard").SendAsync("EventStatsUpdated", eventStats);

                Console.WriteLine($"[Dashboard] EventStatsUpdated sent successfully for event {eventId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dashboard] ERROR in NotifyEventStatsUpdateAsync: {ex.Message}");
                Console.WriteLine($"[Dashboard] StackTrace: {ex.StackTrace}");
            }

        }
    }
}
