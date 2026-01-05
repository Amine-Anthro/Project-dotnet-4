using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
        {
            return await _context.Rooms
                .Include(r => r.Events)
                .ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms
                .Include(r => r.Events)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Room> CreateAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
           
            var room = await _context.Rooms
                .Include(r => r.Seats)
                .Include(r => r.Events)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return false;
            }
           
            if (room.Events.Any())
            {
                throw new InvalidOperationException(
                    "Cannot delete room. It has associated events. Please delete all events first.");
            }

         
            var seatsWithReservations = await _context.Seats
                .Where(s => s.RoomId == id && s.ReservationId != null)
                .CountAsync();

            if (seatsWithReservations > 0)
            {
                throw new InvalidOperationException(
                    "Cannot delete room. Some seats have active reservations.");
            }

            
            if (room.Seats.Any())
            {
                _context.Seats.RemoveRange(room.Seats);
            }

            _context.Rooms.Remove(room);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rooms.AnyAsync(r => r.Id == id);
        }
    }
}
