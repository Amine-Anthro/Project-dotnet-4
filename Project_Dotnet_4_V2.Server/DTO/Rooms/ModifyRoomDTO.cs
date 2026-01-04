namespace Project_Dotnet_4_V2.Server.DTO.Rooms
{
    public class ModifyRoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
       
        public decimal EconomicPrice { get; set; }
        public decimal VipPrice { get; set; }
        public decimal StandardPrice { get; set; }
    }
}
