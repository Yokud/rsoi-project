using DS_Project.Auth.Entity;
using Microsoft.EntityFrameworkCore;

namespace DS_Project.Auth.Repository
{
    public class UsersRepository : IUsersRepository
    {
        readonly UsersDbContext _context;

        public UsersRepository(UsersDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetAsync(Guid guid)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == guid);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            var item = await GetAsync(user.Id);

            if (item is not null)
                _context.Users.Entry(item).CurrentValues.SetValues(user);
        }
    }
}
