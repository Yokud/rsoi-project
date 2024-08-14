using DS_Project.Payments.Entity;
using Microsoft.EntityFrameworkCore;

namespace DS_Project.Payments
{
    public class PaymentsDbContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }

        public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("id");

                entity.HasIndex(e => e.PaymentUid).IsUnique();

                entity.ToTable("payment", t => t.HasCheckConstraint("status", "status IN ('PAID', 'CANCELED')"));

                entity.Property(e => e.Id).ValueGeneratedOnAdd().HasColumnName("id");

                entity.Property(e => e.PaymentUid).HasColumnName("payment_uid");

                entity.Property(e => e.Status).HasMaxLength(20).HasColumnName("status");

                entity.Property(e => e.Price).HasColumnName("price");
            });
        }
    }
}
