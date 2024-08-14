using Microsoft.EntityFrameworkCore;

namespace DS_Project.Cars
{
    public class CarsDbContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        public CarsDbContext(DbContextOptions<CarsDbContext> options) : base(options) 
        {
            Database.EnsureCreated();

            if (Cars is not null && !Cars.Any())
            {
                Cars.Add(new Car
                {
                    Id = 1,
                    CarUid = Guid.Parse("109b42f3-198d-4c89-9276-a7520a7120ab"),
                    Brand = "Mercedes Benz",
                    Model = "GLA 250",
                    RegistrationNumber = "ЛО777Х799",
                    Power = 249,
                    Type = "SEDAN",
                    Price = 3500,
                    Availability = true
                });
                SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("id");

                entity.HasIndex(e => e.CarUid).IsUnique();

                entity.ToTable("cars", t => t.HasCheckConstraint("type", "type IN ('SEDAN', 'SUV', 'MINIVAN', 'ROADSTER')"));

                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");

                entity.Property(e => e.CarUid).HasColumnName("car_uid");

                entity.Property(e => e.Brand).HasMaxLength(80).HasColumnName("brand");

                entity.Property(e => e.Model).HasMaxLength(80).HasColumnName("model");

                entity.Property(e => e.RegistrationNumber).HasMaxLength(20).HasColumnName("registration_number");

                entity.Property(e => e.Power).HasColumnName("power");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Type).HasMaxLength(20).HasColumnName("type");

                entity.Property(e => e.Availability).HasColumnName("availability");
            });
        }
    }
}
