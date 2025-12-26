using Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.DTO.Reservations;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IUserRepository _userRepository; // Tu devras créer celui-ci aussi

        public ReservationService(IReservationRepository reservationRepository, ISeatRepository seatRepository, IUserRepository userRepository)
        {
            _reservationRepository = reservationRepository;
            _seatRepository = seatRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _reservationRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsByUserIdAsync(int userId)
        {
            return await _reservationRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _reservationRepository.GetByIdAsync(id);
        }

        public async Task<Reservation> CreateReservationAsync(CreateReservationDTO reservationDto)
        {
            // 1. Vérifier que l'utilisateur existe
            var user = await _userRepository.GetByIdAsync(reservationDto.UserId);
            if (user == null)
                throw new KeyNotFoundException($"User with id {reservationDto.UserId} not found");

            // 2. Récupérer les sièges demandés
            var seats = await _seatRepository.GetByIdsAsync(reservationDto.SeatIds);
            var seatsList = seats.ToList();

            if (seatsList.Count != reservationDto.SeatIds.Count)
                throw new InvalidOperationException("Some seats were not found");

            // 3. Vérifier que tous les sièges sont disponibles
            var unavailableSeats = seatsList.Where(s => s.Status != SeatStatus.Available).ToList();
            if (unavailableSeats.Any())
            {
                var unavailableIds = string.Join(", ", unavailableSeats.Select(s => $"{s.Row}{s.Number}"));
                throw new InvalidOperationException($"The following seats are not available: {unavailableIds}");
            }

            // 4. Calculer le montant total
            var totalAmount = seatsList.Sum(s => s.Price);

            // 5. Créer la réservation
            var reservation = new Reservation
            {
                UserId = reservationDto.UserId,
                EventId = reservationDto.EventId,
                Status = ReservationStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15), // Réservation temporaire de 15 min
                TotalAmount = totalAmount,
                Seats = seatsList
            };

            var createdReservation = await _reservationRepository.Create(reservation);

            // 6. Marquer les sièges comme réservés (temporaire)
            foreach (var seat in seatsList)
            {
                seat.Status = SeatStatus.Reserved;
                seat.ReservationId = createdReservation.Id;
            }
            await _seatRepository.UpdateRangeAsync(seatsList);

            return createdReservation;
        }

        public async Task<Reservation> ConfirmPaymentAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with id {reservationId} not found");

            if (reservation.Status != ReservationStatus.InProgress)
                throw new InvalidOperationException("Only reservations in progress can be paid");

            // Vérifier que la réservation n'a pas expiré
            if (reservation.ExpiresAt.HasValue && reservation.ExpiresAt.Value < DateTime.UtcNow)
            {
                await CancelReservationAsync(reservationId);
                throw new InvalidOperationException("Reservation has expired");
            }

            // Mettre à jour le statut de la réservation
            reservation.Status = ReservationStatus.Paid;
            reservation.ExpiresAt = null; // Plus besoin d'expiration

            // Mettre à jour les sièges (status = Paid au lieu de Reserved)
            foreach (var seat in reservation.Seats)
            {
                seat.Status = SeatStatus.Paid;
            }

            await _seatRepository.UpdateRangeAsync(reservation.Seats);
            await _reservationRepository.UpdateAsync(reservation);

            return reservation;
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with id {reservationId} not found");

            if (reservation.Status == ReservationStatus.Paid)
                throw new InvalidOperationException("Paid reservations cannot be cancelled");

            // Mettre à jour le statut de la réservation
            reservation.Status = ReservationStatus.Cancelled;

            // Libérer les sièges
            foreach (var seat in reservation.Seats)
            {
                seat.Status = SeatStatus.Available;
                seat.ReservationId = null;
            }

            await _seatRepository.UpdateRangeAsync(reservation.Seats);
            await _reservationRepository.UpdateAsync(reservation);
        }
    }
}
