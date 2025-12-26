using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.DTO.Reservations
{
    public class CreateReservationDTO
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();

    }
}
