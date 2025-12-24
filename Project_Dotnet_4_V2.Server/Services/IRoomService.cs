using Project_Dotnet_4_V2.Server.DTO.Rooms;
using Projet_Dotnet_4.DTO.Rooms;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room?> CreateRoomAsync(CreateRoomDTO roomDto);
        Task UpdateRoomAsync(int id, ModifyRoomDTO roomDto);
        Task DeleteRoomAsync(int id);
       
    }
}
