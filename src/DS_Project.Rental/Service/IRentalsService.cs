using DS_Project.Rentals.Entity;

namespace DS_Project.Rentals.Service
{
    public interface IRentalsService
    {
        Task<IEnumerable<Rental>> GetUserRentalsAsync(string username);
        Task<Rental?> GetAsync(string username, Guid guid);
        Task<Guid?> CreateAsync(Rental rental);
        Task Update(Rental rental);
    }
}
