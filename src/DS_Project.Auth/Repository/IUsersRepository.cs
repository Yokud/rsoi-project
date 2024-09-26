using DS_Project.Auth.Entity;

namespace DS_Project.Auth.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetAsync(Guid guid);
        Task CreateAsync(User user);
        Task Update(User user);
        Task SaveAsync();
    }
}
