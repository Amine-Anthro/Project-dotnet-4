namespace Projet_Dotnet_4.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public  Room? Room { get; set; }
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
    }
}
