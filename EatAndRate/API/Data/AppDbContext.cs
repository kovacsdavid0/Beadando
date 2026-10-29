using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Franchise> Franchises => Set<Franchise>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.Username).HasMaxLength(50).IsRequired();
                e.Property(u => u.Email).HasMaxLength(100).IsRequired();
                e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
                e.HasIndex(u => u.Username).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Restaurant>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Name).HasMaxLength(100).IsRequired();
                e.Property(r => r.Address).HasMaxLength(200);
                e.Property(r => r.City).HasMaxLength(100);
                e.HasOne(r => r.Franchise)
                    .WithMany(f => f.Restaurants)
                    .HasForeignKey(r => r.FranchiseId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Franchise>(e =>
            {
                e.HasKey(f => f.Id);
                e.Property(f => f.Name).HasMaxLength(100).IsRequired();
                e.HasIndex(f => new { f.Name }).IsUnique();
            });

            modelBuilder.Entity<Review>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Comment).HasMaxLength(1000);
                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Review_ServiceRating", "ServiceRating BETWEEN 1 AND 5");
                    t.HasCheckConstraint("CK_Review_FoodRating", "FoodRating BETWEEN 1 AND 5");
                    t.HasCheckConstraint("CK_Review_AtmosphereRating", "AtmosphereRating BETWEEN 1 AND 5");
                });

                e.HasIndex(r => new { r.UserId, r.RestaurantId }).IsUnique();

                e.HasOne(r => r.Restaurant)
                    .WithMany(r => r.Reviews)
                    .HasForeignKey(r => r.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
