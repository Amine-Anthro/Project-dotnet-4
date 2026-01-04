using Microsoft.AspNetCore.SignalR;

namespace Project_Dotnet_4_V2.Server.Hubs
{
    public class EventHub : Hub
    {
        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
        }

        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        }

       
        public async Task JoinEventRoom(int eventId)
        {
            string roomName = $"event-{eventId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            Console.WriteLine($"Client {Context.ConnectionId} joined room: {roomName}");
        }

        
        public async Task LeaveEventRoom(int eventId)
        {
            string roomName = $"event-{eventId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            Console.WriteLine($"Client {Context.ConnectionId} left room: {roomName}");
        }

        public async Task JoinDashboard()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard");
            Console.WriteLine($"Client {Context.ConnectionId} joined dashboard");
        }

        public async Task LeaveDashboard()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "dashboard");
            Console.WriteLine($"Client {Context.ConnectionId} left dashboard");
        }
    }
}

