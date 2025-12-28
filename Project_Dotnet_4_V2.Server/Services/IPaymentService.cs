using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(int reservationId, string paymentMethod);
        Task<Payment?> GetPaymentByReservationIdAsync(int reservationId);
        string GeneratePaymentReference();
    }
}
