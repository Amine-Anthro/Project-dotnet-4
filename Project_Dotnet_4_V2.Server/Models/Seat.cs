namespace Projet_Dotnet_4.Models
{
    public class Seat
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public string Row { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public SeatStatus Status { get; set; } = SeatStatus.Available;
        public decimal Price { get; set; }


    }
}
