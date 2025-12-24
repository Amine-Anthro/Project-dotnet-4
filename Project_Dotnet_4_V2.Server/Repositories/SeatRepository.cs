using Microsoft.EntityFrameworkCore;
using Projet_Dotnet_4.Data;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public class SeatRepository : ISeatRepository
    {
    private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats.FindAsync(id);
        }

        public async Task<IEnumerable<Seat>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Seats
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public async Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Seat> seats)
        {
            _context.Seats.UpdateRange(seats);
            await _context.SaveChangesAsync();
        }
    }
}
