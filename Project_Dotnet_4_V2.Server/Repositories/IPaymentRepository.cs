using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment?> GetByReservationIdAsync(int reservationId);
        Task<Payment> CreateAsync(Payment payment);
        Task UpdateAsync(Payment payment);
    }
}
