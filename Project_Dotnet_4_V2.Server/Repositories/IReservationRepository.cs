using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<IEnumerable<Reservation>> GetAllByUserIdAsync(int userId);
        Task<Reservation?> GetByIdAsync(int id);
        Task<Reservation> Create(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task<bool> ExistsAsync(int id);
    }
}
