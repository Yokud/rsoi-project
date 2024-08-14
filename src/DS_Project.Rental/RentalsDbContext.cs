using Microsoft.EntityFrameworkCore;
using DS_Project.Rentals.Entity;

namespace DS_Project.Rentals
{
    public class RentalsDbContext : DbContext
    {
        public DbSet<Rental> Rentals { get; set; }

        public RentalsDbContext(DbContextOptions<RentalsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("id");

                entity.HasIndex(e => e.RentalUid).IsUnique();

                entity.ToTable("rental", t => t.HasCheckConstraint("status", "status IN ('IN_PROGRESS', 'FINISHED', 'CANCELED')"));

                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");

                entity.Property(e => e.RentalUid).HasColumnName("rental_uid");

                entity.Property(e => e.Username).HasMaxLength(80).HasColumnName("username");

                entity.Property(e => e.PaymentUid).HasColumnName("payment_uid");

                entity.Property(e => e.CarUid).HasColumnName("car_uid");

                entity.Property(e => e.DateFrom).HasColumnName("date_from");

                entity.Property(e => e.DateTo).HasColumnName("date_to");

                entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");
            });
        }
    }
}
