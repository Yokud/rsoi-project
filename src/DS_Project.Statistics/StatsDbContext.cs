using DS_Project.Statistics.Entity;
using Microsoft.EntityFrameworkCore;

namespace DS_Project.Statistics
{
    public class StatsDbContext : DbContext
    {
        public DbSet<Stat> Stats { get; set; }

        public StatsDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stat>(entity =>
            {
                entity.HasKey(x => x.Id).HasName("id");

                entity.ToTable("stats");

                entity.Property(x => x.Id).HasColumnName("id");

                entity.Property(e => e.Text).HasColumnName("text").HasMaxLength(300).IsRequired();
            });
        }
    }
}
