using CloudCompare.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudCompare.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Provider> Providers { get; set; }
        public DbSet<CloudService> CloudServices { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Provider>().HasIndex(p => p.Name).IsUnique();

            mb.Entity<CloudService>().Property(s => s.StartingPrice).HasPrecision(10, 2);
            mb.Entity<CloudService>().Property(s => s.UptimeSla).HasPrecision(6, 3);

            mb.Entity<CloudService>()
              .HasOne(s => s.Provider).WithMany(p => p.Services)
              .HasForeignKey(s => s.ProviderId).OnDelete(DeleteBehavior.Restrict);

            mb.Entity<Review>()
              .HasOne(r => r.CloudService).WithMany(s => s.Reviews)
              .HasForeignKey(r => r.CloudServiceId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
