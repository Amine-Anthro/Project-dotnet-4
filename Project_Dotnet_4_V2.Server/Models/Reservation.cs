namespace Project_Dotnet_4_V2.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public Room? Room { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        public ReservationStatus Status { get; set; } = ReservationStatus.InProgress;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; } 
        public decimal TotalAmount { get; set; }
    }
}
