namespace Projet_Dotnet_4.DTO.Events
{
    public class ModifyEventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public string? Director { get; set; }
        public int? Duration { get; set; }
        public string? Genre { get; set; }
    }
}
