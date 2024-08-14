namespace DS_Project.Cars.Service
{
    public class CarsService : ICarsService
    {
        readonly ICarsRepository _cars;

        public CarsService(ICarsRepository cars)
        {
            _cars = cars;
        }

        public async Task<int> CountAsync()
        {
            return (await _cars.GetAllAsync()).Count();
        }

        public async Task<Guid?> CreateAsync(Car car)
        {
            await _cars.CreateAsync(car);
            await _cars.SaveAsync();

            return car?.CarUid;
        }

        public async Task<Car?> GetAsync(Guid guid)
        {
            return await _cars.GetAsync(guid);
        }

        public async Task<IEnumerable<Car>> GetPageOfAvailableCars(int page, int size, bool showAll)
        {
            return (await _cars.GetAllAsync()).Where(car => car.Availability || showAll).Skip((page - 1) * size).Take(size);
        }

        public async Task UpdateAsync(Car car)
        {
            await _cars.Update(car);
            await _cars.SaveAsync();
        }
    }
}
