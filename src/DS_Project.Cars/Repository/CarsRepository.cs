using Microsoft.EntityFrameworkCore;

namespace DS_Project.Cars
{
    public class CarsRepository : ICarsRepository
    {
        readonly CarsDbContext _context;

        public CarsRepository(CarsDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Car car)
        {
            await _context.Cars.AddAsync(car);
        }

        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            return await _context.Cars.ToListAsync();
        }

        public async Task<Car?> GetAsync(Guid guid)
        {
            return await _context.Cars.FirstOrDefaultAsync(c => c.CarUid == guid);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(Car car)
        {
            var item = await GetAsync(car.CarUid);

            if (item is not null)
                _context.Cars.Entry(item).CurrentValues.SetValues(car);
        }
    }
}
