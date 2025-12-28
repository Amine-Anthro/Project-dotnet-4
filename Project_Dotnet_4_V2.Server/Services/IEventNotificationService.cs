namespace Project_Dotnet_4_V2.Server.Services
{
    public interface IEventNotificationService
    {
        Task NotifySeatReservedAsync(int eventId, int seatId, string row, string number);
        Task NotifySeatReleasedAsync(int eventId, int seatId, string row, string number);
        Task NotifySeatPaidAsync(int eventId, int seatId, string row, string number);
    }
}
