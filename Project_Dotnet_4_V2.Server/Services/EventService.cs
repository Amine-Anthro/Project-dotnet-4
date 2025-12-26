using Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.Server.Repositories.Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.DTO.Events;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IRoomRepository _roomRepository;

        public EventService(IEventRepository eventRepository, IRoomRepository roomRepository)
        {
            _eventRepository = eventRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllAsync();
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            return await _eventRepository.GetByIdAsync(id);
        }

        public async Task<Event> CreateEventAsync(CreateEventDTO eventDto)
        {
            // Vérifier que la salle existe
            if (!await _roomRepository.ExistsAsync(eventDto.RoomId))
            {
                throw new KeyNotFoundException($"Room with id {eventDto.RoomId} not found");
            }

            var eventItem = new Event
            {
                Name = eventDto.Name,
                Date = eventDto.Date,
                Description = eventDto.Description,
                RoomId = eventDto.RoomId,
                ImageUrl = eventDto.ImageUrl,
                BasePrice = eventDto.BasePrice,
                Director = eventDto.Director,
                Duration = eventDto.Duration,
                Genre = eventDto.Genre
            };

            return await _eventRepository.CreateAsync(eventItem);
        }

        public async Task UpdateEventAsync(int id, ModifyEventDTO eventDto)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);

            if (eventItem == null)
                throw new KeyNotFoundException($"Event with id {id} not found");

            // Vérifier que la nouvelle salle existe si elle a changé
            if (eventDto.RoomId != eventItem.RoomId)
            {
                if (!await _roomRepository.ExistsAsync(eventDto.RoomId))
                {
                    throw new KeyNotFoundException($"Room with id {eventDto.RoomId} not found");
                }
            }

            eventItem.Name = eventDto.Name;
            eventItem.Date = eventDto.Date;
            eventItem.Description = eventDto.Description;
            eventItem.RoomId = eventDto.RoomId;
            eventItem.ImageUrl = eventDto.ImageUrl;
            eventItem.BasePrice = eventDto.BasePrice;
            eventItem.Director = eventDto.Director;
            eventItem.Duration = eventDto.Duration;
            eventItem.Genre = eventDto.Genre;

            await _eventRepository.UpdateAsync(eventItem);
        }

        public async Task DeleteEventAsync(int id)
        {
            if (!await _eventRepository.ExistsAsync(id))
                throw new KeyNotFoundException($"Event with id {id} not found");

            await _eventRepository.DeleteAsync(id);
        }
    }
}