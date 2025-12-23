using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Server.DTO.Rooms;
using Projet_Dotnet_4.Data;
using Projet_Dotnet_4.DTO.Rooms;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(e => e.Events)
                .ToListAsync();

            return Ok(rooms);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomById(int id)
        {
            var roomItem = await _context.Rooms
                .Include(e => e.Events)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (roomItem == null)
            {
                return NotFound();
            }

            return Ok(roomItem);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom(CreateRoomDTO roomDto)
        {
            var room = new Room
            {
                Name = roomDto.Name,
                Capacity = roomDto.Capacity,
                EconomicPrice = roomDto.EconomicPrice,
                VipPrice = roomDto.VipPrice,
                StandardPrice = roomDto.StandardPrice,
                TotalRows = roomDto.TotalRows,
                SeatsPerRow = roomDto.SeatsPerRow
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            GenerateSeats(room);

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, room);
        }

        private void GenerateSeats(Room room)
        {
            int totalRows = room.TotalRows;
            int seatsPerRow = room.SeatsPerRow;

            int firstThird = totalRows / 3;
            int middleThird = firstThird * 2;

            for (int rowIndex = 0; rowIndex < totalRows; rowIndex++)
            {
                char rowLetter = (char)('A' + rowIndex);

                TariffZone zone;
                decimal price;

                if (rowIndex < firstThird)
                {
                    zone = TariffZone.Economic;
                    price = room.EconomicPrice;

                }
                else if (rowIndex < middleThird)
                {
                    zone = TariffZone.VIP;
                    price = room.VipPrice;
                }
                else
                {
                    zone = TariffZone.Standard;
                    price = room.StandardPrice;
                }

                for (int seatNumber = 1; seatNumber <= seatsPerRow; seatNumber++)
                {
                    var seat = new Seat
                    {
                        Number = seatNumber.ToString(),
                        Row = rowLetter.ToString(),
                        RoomId = room.Id,
                        Status = SeatStatus.Available,
                        Zone = zone,
                        Price = price
                    };
                    _context.Seats.Add(seat);
                }
            }
            _context.SaveChanges();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifyRoom(int id, ModifyRoomDTO roomDto)
        {
            if (id != roomDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }
            room.Name = roomDto.Name;
            room.Capacity = roomDto.Capacity;
            room.EconomicPrice = roomDto.EconomicPrice;
            room.VipPrice = roomDto.VipPrice;
            room.StandardPrice = roomDto.StandardPrice;
            room.TotalRows = roomDto.TotalRows;
            room.SeatsPerRow = roomDto.SeatsPerRow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
