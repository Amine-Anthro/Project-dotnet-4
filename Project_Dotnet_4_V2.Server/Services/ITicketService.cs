namespace Project_Dotnet_4_V2.Server.Services
{
    public interface ITicketService
    {
        Task<byte[]> GenerateTicketPdfAsync(int reservationId);
        Task<string> GenerateQRCodeAsync(int reservationId);
    }
}
