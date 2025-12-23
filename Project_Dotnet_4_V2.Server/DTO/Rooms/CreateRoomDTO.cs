namespace Projet_Dotnet_4.DTO.Rooms
{
    public class CreateRoomDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
        public int TotalRows { get; set; } 
        public int SeatsPerRow { get; set; }
        public decimal EconomicPrice { get; set; }
        public decimal VipPrice { get; set; }
        public decimal StandardPrice { get; set; }
    }
}
