using DS_Project.Rentals.Entity;

namespace DS_Project.Rentals.Repository
{
    public interface IRentalsRepository
    {
        Task<IEnumerable<Rental>> GetAllAsync();
        Task<Rental?> GetAsync(Guid guid);
        Task CreateAsync(Rental rental);
        Task Update(Rental rental);
        Task SaveAsync();
    }
}
