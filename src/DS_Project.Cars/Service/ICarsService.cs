using Microsoft.AspNetCore.Components;

namespace DS_Project.Cars
{
    public interface ICarsService
    {
        Task<Car?> GetAsync(Guid guid);
        Task<Guid?> CreateAsync(Car car);
        Task UpdateAsync(Car car);
        Task<IEnumerable<Car>> GetPageOfAvailableCars(int page, int size, bool showAll);
        Task<int> CountAsync();
    }
}
