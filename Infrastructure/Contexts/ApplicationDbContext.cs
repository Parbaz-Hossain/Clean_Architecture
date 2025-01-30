using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
                entity.HasIndex(p => p.Name).IsUnique();
                entity.Property(p => p.Price).IsRequired().HasPrecision(18, 6);
            });
        }
    }
}
