using DS_Project.Payments.Entity;

namespace DS_Project.Payments.Repository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<Payment?> GetAsync(Guid guid);
        Task CreateAsync(Payment payment);
        Task Update(Payment payment);
        Task SaveAsync();
    }
}
