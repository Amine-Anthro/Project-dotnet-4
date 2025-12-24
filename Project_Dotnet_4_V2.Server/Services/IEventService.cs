using Projet_Dotnet_4.DTO.Events;
using Projet_Dotnet_4.Models;

namespace Project_Dotnet_4_V2.Server.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(CreateEventDTO eventDto);
        Task UpdateEventAsync(int id, ModifyEventDTO eventDto);
        Task DeleteEventAsync(int id);


    }
}