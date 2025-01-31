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

                #region Seed Product Data

                entity.HasData(new Product
                {
                    Id = 2,
                    Name = "IPhone 14 Pro Max",
                    Price = 130000
                },
                new Product
                {
                    Id = 3,
                    Name = "IPhone 15 Pro Max",
                    Price = 140000
                },
                new Product
                {
                    Id = 4,
                    Name = "IPhone 16 Pro Mini",
                    Price = 150000
                });

                #endregion
            });
        }
    }
}
