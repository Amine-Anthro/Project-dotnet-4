namespace Projet_Dotnet_4.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SeatsPerRow { get; set; }
        public decimal EconomicPrice { get; set; }
        public decimal VipPrice { get; set; }
        public decimal StandardPrice { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Event> Events { get; set; } = new List<Event>();

    }
}
