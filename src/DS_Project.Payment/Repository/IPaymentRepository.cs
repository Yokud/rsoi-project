using DS_Project.Payments.Entity;

namespace DS_Project.Payments.Repository
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Entity.Payment>> GetAllAsync();
        Task<Entity.Payment?> GetAsync(Guid guid);
        Task CreateAsync(Entity.Payment payment);
        Task Update(Entity.Payment payment);
        Task SaveAsync();
    }
}
