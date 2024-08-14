using DS_Project.Payments.Entity;

namespace DS_Project.Payments.Service
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetAsync(Guid guid);
        Task<Guid?> CreateAsync(Payment payment);
        Task Update(Payment payment);
    }
}
