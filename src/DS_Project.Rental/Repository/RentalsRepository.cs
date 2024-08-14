using DS_Project.Rentals.Entity;
using Microsoft.EntityFrameworkCore;

namespace DS_Project.Rentals.Repository
{
    public class RentalsRepository : IRentalsRepository
    {
        readonly RentalsDbContext _context;

        public RentalsRepository(RentalsDbContext rentals)
        {
            _context = rentals;
        }

        public async Task CreateAsync(Rental rental)
        {
            await _context.Rentals.AddAsync(rental);
        }

        public async Task DeleteAsync(Guid guid)
        {
            var item = await GetAsync(guid);

            if (item is not null)
                _context.Rentals.Remove(item);
        }

        public async Task<IEnumerable<Rental>> GetAllAsync()
        {
            return await _context.Rentals.ToListAsync();
        }

        public async Task<Rental?> GetAsync(Guid guid)
        {
            return await _context.Rentals.FirstOrDefaultAsync(c => c.RentalUid == guid);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Rental rental)
        {
            var item = await GetAsync(rental.RentalUid);

            if (item is not null)
                _context.Rentals.Entry(item).CurrentValues.SetValues(rental);
        }
    }
}
