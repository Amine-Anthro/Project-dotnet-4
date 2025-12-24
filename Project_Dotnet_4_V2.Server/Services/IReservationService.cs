using Projet_Dotnet_4.DTO.Reservations;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<IEnumerable<Reservation>> GetAllReservationsByUserIdAsync(int userId);
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<Reservation> CreateReservationAsync(CreateReservationDTO reservationDto);
        Task<Reservation> ConfirmPaymentAsync(int reservationId);
        Task CancelReservationAsync(int reservationId);
    }
}
