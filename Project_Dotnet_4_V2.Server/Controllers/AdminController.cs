using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Models;
using Project_Dotnet_4_V2.Server.Models;
using Project_Dotnet_4_V2.Server.Services;

namespace Project_Dotnet_4_V2.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;

        public AdminController(ApplicationDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpPost("migrate-payments")]
        public async Task<IActionResult> MigratePayments()
        {
            var paidReservationsWithoutPayment = await _context.Reservations
                .Include(r => r.Payment)
                .Where(r => r.Status == ReservationStatus.Paid && r.Payment == null)
                .ToListAsync();

            int count = 0;

            foreach (var reservation in paidReservationsWithoutPayment)
            {
                var payment = new Payment
                {
                    ReservationId = reservation.Id,
                    Amount = reservation.TotalAmount,
                    Status = PaymentStatus.Completed,
                    Reference = _paymentService.GeneratePaymentReference(),
                    PaymentMethod = "card",
                    TransactionId = Guid.NewGuid().ToString(),
                    CreatedAt = reservation.CreatedAt,
                    CompletedAt = reservation.CreatedAt
                };

                _context.Payments.Add(payment);
                count++;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Migration completed: {count} payments created",
                count = count
            });
        }
    }
}
