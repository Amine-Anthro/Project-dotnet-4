using Project_Dotnet_4_V2.Server.Models;

namespace Project_Dotnet_4_V2.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public string PaymentMethod { get; set; } = "card";
        public string? TransactionId { get; set; }
    }
}
