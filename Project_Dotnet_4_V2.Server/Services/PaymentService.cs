using Project_Dotnet_4_V2.Models;
using Project_Dotnet_4_V2.Server.Models;
using Project_Dotnet_4_V2.Server.Repositories;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReservationRepository _reservationRepository;

        public PaymentService(IPaymentRepository paymentRepository, IReservationRepository reservationRepository)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
        }

        public string GeneratePaymentReference()
        {
            // Générer une référence unique
            return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }

        public async Task<Payment> ProcessPaymentAsync(int reservationId, string paymentMethod)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);

            if (reservation == null)
            {
                throw new KeyNotFoundException($"Reservation {reservationId} not found");
            }
            if (reservation.Status != ReservationStatus.InProgress)
            {
                throw new InvalidOperationException("Only reservations in progress can be paid");
            }
            // Vérifier si un paiement existe déjà
            var existingPayment = await _paymentRepository.GetByReservationIdAsync(reservationId);
            if (existingPayment != null && existingPayment.Status == PaymentStatus.Completed)
                throw new InvalidOperationException("This reservation has already been paid");

            // Créer le paiement
            var payment = new Payment
            {
                ReservationId = reservationId,
                Amount = reservation.TotalAmount,
                Status = PaymentStatus.Completed,
                Reference = GeneratePaymentReference(),
                PaymentMethod = paymentMethod,
                TransactionId = Guid.NewGuid().ToString(), // ID de transaction fictif
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow
            };

            await _paymentRepository.CreateAsync(payment);

            return payment;
        }

        public async Task<Payment?> GetPaymentByReservationIdAsync(int reservationId)
        {
            return await _paymentRepository.GetByReservationIdAsync(reservationId);
        }
    }
}
