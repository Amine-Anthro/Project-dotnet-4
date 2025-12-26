using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public interface ISeatRepository
    {
        Task<Seat?> GetByIdAsync(int id);
        Task<IEnumerable<Seat>> GetByIdsAsync(List<int> ids);
        Task UpdateAsync(Seat seat);
        Task UpdateRangeAsync(IEnumerable<Seat> seats);

    }
}
