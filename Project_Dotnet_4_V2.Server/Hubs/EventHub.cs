using Microsoft.AspNetCore.SignalR;

namespace Project_Dotnet_4_V2.Server.Hubs
{
    public class EventHub : Hub
    {
        // Méthode appelée quand un client se connecte
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
        }

        // Méthode appelée quand un client se déconnecte
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        }

        // Rejoindre un groupe pour un événement spécifique
        public async Task JoinEventRoom(int eventId)
        {
            string roomName = $"event-{eventId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            Console.WriteLine($"Client {Context.ConnectionId} joined room: {roomName}");
        }

        // Quitter un groupe
        public async Task LeaveEventRoom(int eventId)
        {
            string roomName = $"event-{eventId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            Console.WriteLine($"Client {Context.ConnectionId} left room: {roomName}");
        }
    }
}

