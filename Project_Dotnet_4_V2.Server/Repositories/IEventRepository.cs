using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    namespace Project_Dotnet_4_V2.Server.Repositories
    {
        public interface IEventRepository
        {
            Task<IEnumerable<Event>> GetAllAsync();
            Task<Event?> GetByIdAsync(int id);
            Task<Event> CreateAsync(Event eventItem);
            Task UpdateAsync(Event eventItem);
            Task DeleteAsync(int id);
            Task<bool> ExistsAsync(int id);
        }
    }
}
