using DS_Project.Auth.Entity;
using Microsoft.EntityFrameworkCore;

namespace DS_Project.Auth
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
            Database.EnsureCreated();

            if (Users is not null && !Users.Any())
            {
                Users.Add(new User()
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    Password = "admin",
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "admin@mail.com",
                    PhoneNumber = "88005553535",
                    Role = UserRole.Admin.GetRoleNameString(),
                });
                SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id).HasName("id");

                entity.HasIndex(x => x.Id).IsUnique();

                entity.ToTable("users", t => t.HasCheckConstraint("role", "role IN ('user', 'admin')"));

                entity.Property(x => x.Id).HasColumnName("id");

                entity.Property(x => x.UserName).HasMaxLength(128).HasColumnName("username");

                entity.Property(x => x.Password).HasColumnName("password");

                entity.Property(x => x.PhoneNumber).HasColumnName("phone_number");

                entity.Property(x => x.Email).HasColumnName("email");

                entity.Property(x => x.FirstName).HasColumnName("first_name");

                entity.Property(x => x.LastName).HasColumnName("last_name");

                entity.Property(x => x.Role).HasDefaultValue("user").HasColumnName("role");
            });
        }
    }
}
