using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Models;
using Project_Dotnet_4_V2.Server.Repositories;

namespace Project_Dotnet_4_V2.Server.Services
{
    public class ReservationExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationExpirationService> _logger;
        

        public ReservationExpirationService(
            IServiceProvider serviceProvider,
            ILogger<ReservationExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Expiration Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpiredReservationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking expired reservations");
                }

               
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            _logger.LogInformation(" Reservation Expiration Service stopped");
        }

        private async Task CheckExpiredReservationsAsync()
        {
           
            using var scope = _serviceProvider.CreateScope();
            var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();
            await reservationService.CancelExpiredReservationsAsync();

            _logger.LogInformation(" Expired reservations checked");
        }


    }
}
