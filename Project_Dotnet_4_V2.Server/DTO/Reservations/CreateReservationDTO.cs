using Projet_Dotnet_4.Models;

namespace Projet_Dotnet_4.DTO.Reservations
{
    public class CreateReservationDTO
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public List<int> SeatIds { get; set; } = new List<int>();

    }
}
