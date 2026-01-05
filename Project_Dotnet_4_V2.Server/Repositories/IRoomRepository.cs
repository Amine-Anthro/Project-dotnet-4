using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int id);
        Task<Room> CreateAsync(Room room);
        Task UpdateAsync(Room room);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
