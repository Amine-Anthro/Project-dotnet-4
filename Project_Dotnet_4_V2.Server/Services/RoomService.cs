using Project_Dotnet_4_V2.Server.DTO.Rooms;
using Project_Dotnet_4_V2.Server.Repositories;
using Projet_Dotnet_4.Data;
using Projet_Dotnet_4.DTO.Rooms;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ApplicationDbContext _context;

        public RoomService(IRoomRepository roomRepository, ApplicationDbContext context)
        {
            _roomRepository = roomRepository;
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _roomRepository.GetAllAsync();
        }

        public async Task<Room?> GetRoomByIdAsync(int id)
        {
            return await _roomRepository.GetByIdAsync(id);
        }

        public async Task<Room> CreateRoomAsync(CreateRoomDTO roomDto)
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

            await _roomRepository.CreateAsync(room);
            GenerateSeats(room);

            return room;
        }

        public async Task UpdateRoomAsync(int id, ModifyRoomDTO roomDto)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
                throw new KeyNotFoundException($"Room with id {id} not found");

            room.Name = roomDto.Name;
            room.Capacity = roomDto.Capacity;
            room.EconomicPrice = roomDto.EconomicPrice;
            room.VipPrice = roomDto.VipPrice;
            room.StandardPrice = roomDto.StandardPrice;
            room.TotalRows = roomDto.TotalRows;
            room.SeatsPerRow = roomDto.SeatsPerRow;

            await _roomRepository.UpdateAsync(room);
        }

        public async Task DeleteRoomAsync(int id)
        {
            if (!await _roomRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Room with id {id} not found");

            await _roomRepository.DeleteAsync(id);
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
    }
}
