using DS_Project.Payments.Entity;
using Microsoft.EntityFrameworkCore;

namespace DS_Project.Payments.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        readonly PaymentsDbContext _context;

        public PaymentRepository(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task<Payment?> GetAsync(Guid guid)
        {
            return await _context.Payments.FirstOrDefaultAsync(c => c.PaymentUid == guid);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Payment payment)
        {
            var item = await GetAsync(payment.PaymentUid);

            if (item is not null)
                _context.Payments.Entry(item).CurrentValues.SetValues(payment);
        }
    }
}
