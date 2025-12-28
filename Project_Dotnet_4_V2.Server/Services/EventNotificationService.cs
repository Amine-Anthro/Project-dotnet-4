using Microsoft.AspNetCore.SignalR;
using Project_Dotnet_4_V2.Server.Hubs;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class EventNotificationService : IEventNotificationService
    {
        private readonly IHubContext<EventHub> _hubContext;

        public EventNotificationService(IHubContext<EventHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySeatReservedAsync(int eventId, int seatId, string row, string number)
        {
            await _hubContext.Clients.Group($"event-{eventId}").SendAsync("SeatReserved", new
            {
                seatId,
                row,
                number,
                status = "Reserved"
            });
        }

        public async Task NotifySeatReleasedAsync(int eventId, int seatId, string row, string number)
        {
            await _hubContext.Clients.Group($"event-{eventId}").SendAsync("SeatReleased", new
            {
                seatId,
                row,
                number,
                status = "Available"
            });
        }

        public async Task NotifySeatPaidAsync(int eventId, int seatId, string row, string number)
        {
            await _hubContext.Clients.Group($"event-{eventId}").SendAsync("SeatPaid", new
            {
                seatId,
                row,
                number,
                status = "Paid"
            });
        }
    }
}
