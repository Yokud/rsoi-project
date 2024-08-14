using DS_Project.Rentals.Entity;
using DS_Project.Rentals.Repository;

namespace DS_Project.Rentals.Service
{
    public class RentalsService : IRentalsService
    {
        readonly IRentalsRepository _rentals;

        public RentalsService(IRentalsRepository rentals)
        {
            _rentals = rentals;
        }

        public async Task<Guid?> CreateAsync(Rental rental)
        {
            await _rentals.CreateAsync(rental);
            await _rentals.SaveAsync();

            return rental.RentalUid;
        }

        public async Task<Rental?> GetAsync(string username, Guid guid)
        {
            var item = await _rentals.GetAsync(guid);

            return item?.Username == username ? item : null;
        }

        public async Task<IEnumerable<Rental>> GetUserRentalsAsync(string username)
        {
            return (await _rentals.GetAllAsync()).Where(r => r.Username == username);
        }

        public async Task Update(Rental rental)
        {
            await _rentals.Update(rental);
            await _rentals.SaveAsync();
        }
    }
}
