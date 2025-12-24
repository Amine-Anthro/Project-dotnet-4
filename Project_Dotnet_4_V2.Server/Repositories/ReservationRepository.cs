using Microsoft.EntityFrameworkCore;
using Projet_Dotnet_4.Data;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Reservation> Create(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Reservations.AnyAsync(r => r.Id == id);
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
           return await _context.Reservations.Include(r => r.User).Include(r => r.Seats).ToListAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Reservations.Include(r => r.User).Include(r => r.Seats).Where(r => r.UserId == userId).ToListAsync();
        }
    }
}
