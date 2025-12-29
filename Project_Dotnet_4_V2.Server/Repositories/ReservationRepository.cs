using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Models;

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
                .Include(r => r.Event)
                 .ThenInclude(e => e.Room)
                .Include(r => r.Seats)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
           return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Event)
                 .ThenInclude(e => e.Room)
                 .Include(r => r.Payment)
                .Include(r => r.Seats)
                .ToListAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reservation>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Seats)
                .Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> CheckExpirationAsync()
        {
            return await _context.Reservations
                    .Include(r => r.Seats)
                    .Where(r =>
                        r.Status == ReservationStatus.InProgress &&
                        r.ExpiresAt.HasValue &&
                        r.ExpiresAt.Value < DateTime.UtcNow)
                    .ToListAsync();
        }

        public async Task<int> CountActiveReservationsAsync()
        {
            return await _context.Reservations
                .CountAsync(r => r.Status == ReservationStatus.InProgress || r.Status == ReservationStatus.Paid);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByEventIdAsync(int eventId)
        {
            return await _context.Reservations
                .Include(r => r.Seats)
                .Where(r => r.EventId == eventId)
                .ToListAsync();
        }
    }
}
