using DS_Project.Payments.Entity;
using DS_Project.Payments.Repository;

namespace DS_Project.Payments.Service
{
    public class PaymentService : IPaymentService
    {
        readonly IPaymentRepository _payments;

        public PaymentService(IPaymentRepository payments)
        {
            _payments = payments;
        }

        public async Task<Guid?> CreateAsync(Payment payment)
        {
            await _payments.CreateAsync(payment);
            await _payments.SaveAsync();

            return payment?.PaymentUid;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _payments.GetAllAsync();
        }

        public async Task<Payment?> GetAsync(Guid guid)
        {
            return await _payments.GetAsync(guid);
        }

        public async Task Update(Payment payment)
        {
            await _payments.Update(payment);
            await _payments.SaveAsync();
        }
    }
}
