using DS_Project.Payments.Entity;

namespace DS_Project.Payments.Service
{
    public interface IPaymentService
    {
        Task<IEnumerable<Entity.Payment>> GetAllAsync();
        Task<Entity.Payment?> GetAsync(Guid guid);
        Task<Guid?> CreateAsync(Entity.Payment payment);
        Task Update(Entity.Payment payment);
    }
}
