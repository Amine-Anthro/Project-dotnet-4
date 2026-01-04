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
        private readonly IUserRepository _userRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEventNotificationService _notificationService;
        public ReservationService(IReservationRepository reservationRepository, ISeatRepository seatRepository, IUserRepository userRepository, IPaymentService paymentService, IEventNotificationService notificationService)
        {
            _reservationRepository = reservationRepository;
            _seatRepository = seatRepository;
            _userRepository = userRepository;
            _paymentService = paymentService;
            _notificationService = notificationService;
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
            
            var user = await _userRepository.GetByIdAsync(reservationDto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {reservationDto.UserId} not found");
            }
            var seats = await _seatRepository.GetByIdsAsync(reservationDto.SeatIds);
            var seatsList = seats.ToList();

            if (seatsList.Count != reservationDto.SeatIds.Count)
            {
                throw new InvalidOperationException("Some seats were not found");
            }
         
            var unavailableSeats = seatsList.Where(s => s.Status != SeatStatus.Available).ToList();
            if (unavailableSeats.Any())
            {
                var unavailableIds = string.Join(", ", unavailableSeats.Select(s => $"{s.Row}{s.Number}"));
                throw new InvalidOperationException($"The following seats are not available: {unavailableIds}");
            }

           
            var totalAmount = seatsList.Sum(s => s.Price);

            
            var reservation = new Reservation
            {
                UserId = reservationDto.UserId,
                EventId = reservationDto.EventId,
                Status = ReservationStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                TotalAmount = totalAmount,
                Seats = seatsList
            };

            var createdReservation = await _reservationRepository.Create(reservation);

            
            foreach (var seat in seatsList)
            {
                seat.Status = SeatStatus.Reserved;
                seat.ReservationId = createdReservation.Id;
            }

            
            await _seatRepository.UpdateRangeAsync(seatsList);

            
            foreach (var seat in seatsList)
            {
                await _notificationService.NotifySeatReservedAsync(
                   reservationDto.EventId,
                   seat.Id,
                   seat.Row,
                   seat.Number
                );
            }

            return createdReservation;
        }

        public async Task<Reservation> ConfirmPaymentAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation with id {reservationId} not found");
            }
            if (reservation.Status != ReservationStatus.InProgress)
                throw new InvalidOperationException("Only reservations in progress can be paid");

            if (reservation.ExpiresAt.HasValue && reservation.ExpiresAt.Value < DateTime.UtcNow)
            {
                await CancelReservationAsync(reservationId);
                throw new InvalidOperationException("Reservation has expired");
            }

            var payment = await _paymentService.ProcessPaymentAsync(reservationId, "card");

            reservation.Status = ReservationStatus.Paid;
            reservation.ExpiresAt = null;

            
            foreach (var seat in reservation.Seats)
            {
                seat.Status = SeatStatus.Paid;
            }

            
            await _seatRepository.UpdateRangeAsync(reservation.Seats);
            await _reservationRepository.UpdateAsync(reservation);

            
            foreach (var seat in reservation.Seats)
            {
                await _notificationService.NotifySeatPaidAsync(
                      reservation.EventId,
                      seat.Id,
                      seat.Row,
                      seat.Number
                );
            }

            return reservation;
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
                throw new KeyNotFoundException($"Reservation with id {reservationId} not found");

            if (reservation.Status == ReservationStatus.Paid)
                throw new InvalidOperationException("Paid reservations cannot be cancelled");

            reservation.Status = ReservationStatus.Cancelled;

            foreach (var seat in reservation.Seats)
            {
                seat.Status = SeatStatus.Available;
                seat.ReservationId = null;
            }

            
            await _seatRepository.UpdateRangeAsync(reservation.Seats);
            await _reservationRepository.UpdateAsync(reservation);

            foreach (var seat in reservation.Seats)
            {
                await _notificationService.NotifySeatReleasedAsync(
                      reservation.EventId,
                      seat.Id,
                      seat.Row,
                      seat.Number
                );
            }
        }

        public async Task CancelExpiredReservationsAsync()
        {
            var expiredReservations = await _reservationRepository.CheckExpirationAsync();

            if (!expiredReservations.Any())
                return;

            foreach (var reservation in expiredReservations)
            {
                reservation.Status = ReservationStatus.Cancelled;

                foreach (var seat in reservation.Seats)
                {
                    seat.Status = SeatStatus.Available;
                    seat.ReservationId = null;
                }

                await _seatRepository.UpdateRangeAsync(reservation.Seats);
                await _reservationRepository.UpdateAsync(reservation);

                foreach (var seat in reservation.Seats)
                {
                    await _notificationService.NotifySeatReleasedAsync(
                        reservation.EventId,
                        seat.Id,
                        seat.Row,
                        seat.Number
                    );
                }
            }
        }
        }
    }
