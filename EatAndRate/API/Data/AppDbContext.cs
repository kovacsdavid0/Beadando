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
                e.Property(u => u.Username).HasMaxLength(50);
                e.Property(u => u.Email).HasMaxLength(100);
                e.Property(u => u.PasswordHash).HasMaxLength(255);
                e.HasIndex(u => u.Username).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Restaurant>(e =>
            {
                e.Property(r => r.Name).HasMaxLength(100);
                e.Property(r => r.Address).HasMaxLength(200);
                e.Property(r => r.City).HasMaxLength(100);
                e.Property(r => r.FranchiseId).HasMaxLength(100);
            });

            modelBuilder.Entity<Franchise>(e =>
            {
                e.Property(r => r.Name).HasMaxLength(100);
                e.HasIndex(r => new { r.FranchiseId }).IsUnique();
                e.HasIndex(r => new { r.Name }).IsUnique();
            });

            modelBuilder.Entity<Review>(e =>
            {
                e.Property(r => r.Comment).HasMaxLength(1000);
                e.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "Rating BETWEEN 1 AND 5"));

                // Egy felhasználó egy éttermet csak egyszer értékelhet
                e.HasIndex(r => new { r.UserId, r.RestaurantId }).IsUnique();

                e.HasOne(r => r.Restaurant)
                    .WithMany(r => r.Reviews)
                    .HasForeignKey(r => r.RestaurantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
