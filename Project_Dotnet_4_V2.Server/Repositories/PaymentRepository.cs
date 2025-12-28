using Microsoft.EntityFrameworkCore;
using Project_Dotnet_4_V2.Data;
using Project_Dotnet_4_V2.Models;

namespace Project_Dotnet_4_V2.Server.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment?> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Payments
                .Include(p => p.Reservation)
                .FirstOrDefaultAsync(p => p.ReservationId == reservationId);
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
