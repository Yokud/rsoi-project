namespace DS_Project.Cars
{
    public interface ICarsRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetAsync(Guid guid);
        Task CreateAsync(Car car);
        Task Update(Car car);
        Task SaveAsync();
    }
}
