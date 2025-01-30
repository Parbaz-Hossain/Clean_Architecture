using Domain.Entities.Products;
using Domain.Interfaces.Products;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Products
{
    public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
        }
    }
}
