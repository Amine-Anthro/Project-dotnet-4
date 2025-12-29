using Project_Dotnet_4_V2.Models;
using Project_Dotnet_4_V2.Server.DTO.Dashboard;
using Project_Dotnet_4_V2.Server.Repositories;
using Project_Dotnet_4_V2.Server.Repositories.Project_Dotnet_4_V2.Server.Repositories;
using System.Diagnostics.Tracing;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IReservationRepository _reservationRepository;

        public DashboardService( IEventRepository eventRepository,IReservationRepository reservationRepository)
        {
            _eventRepository = eventRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync(int? eventId = null)
        {
           
            var events = await _eventRepository.GetAllWithRoomAndSeatsAsync();
            var eventsList = events.ToList();

            if (eventId.HasValue)
            {
                eventsList = eventsList.Where(e => e.Id == eventId.Value).ToList();
            }

            var stats = new DashboardStatsDTO
            {
                TotalEvents = eventsList.Count,
                EventsStats = new List<EventStatsDTO>()
            };

            foreach (var evt in eventsList)
            {
                var eventStats = await GetEventStatsAsync(evt.Id);
                stats.EventsStats.Add(eventStats);

                stats.TotalSeatsReserved += eventStats.ReservedSeats;
                stats.TotalSeatsPaid += eventStats.PaidSeats;
                stats.TotalRevenue += eventStats.Revenue;
                stats.PendingRevenue += eventStats.PotentialRevenue - eventStats.Revenue;
            }
            stats.TotalReservations = await _reservationRepository.CountActiveReservationsAsync();

            return stats;
        }

        public async Task<EventStatsDTO> GetEventStatsAsync(int eventId)
        {
            
            var evt = await _eventRepository.GetByIdAsync(eventId);

            if (evt == null || evt.Room == null)
                return new EventStatsDTO { EventId = eventId, EventName = "Unknown" };

            var seats = evt.Room.Seats.ToList();
            var totalSeats = seats.Count;
            var availableSeats = seats.Count(s => s.Status == SeatStatus.Available);
            var reservedSeats = seats.Count(s => s.Status == SeatStatus.Reserved);
            var paidSeats = seats.Count(s => s.Status == SeatStatus.Paid);

            var revenue = seats
                .Where(s => s.Status == SeatStatus.Paid)
                .Sum(s => s.Price);

            var potentialRevenue = seats.Sum(s => s.Price);

            var fillRate = totalSeats > 0
                ? (decimal)(reservedSeats + paidSeats) / totalSeats * 100
                : 0;

            return new EventStatsDTO
            {
                EventId = evt.Id,
                EventName = evt.Name,
                TotalSeats = totalSeats,
                AvailableSeats = availableSeats,
                ReservedSeats = reservedSeats,
                PaidSeats = paidSeats,
                FillRate = Math.Round(fillRate, 2),
                Revenue = revenue,
                PotentialRevenue = potentialRevenue
            };
        }
    }
}
