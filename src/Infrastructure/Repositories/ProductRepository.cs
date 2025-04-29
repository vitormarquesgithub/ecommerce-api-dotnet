using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Models;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories {
    public class ProductRepository : IProductRepository {
        private readonly ECommerceDbContext _context;

        public ProductRepository(ECommerceDbContext context) {
            _context = context;
        }

        public async Task<Product?> GetProductById(Guid id)
            => await _context.Products.FindAsync(id);

        public async Task<IEnumerable<Product>> GetAllProducts()
            => await _context.Products
                .AsNoTracking()
                .ToListAsync();

        public async Task<Product> AddProduct(Product Product) {
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();
            return Product;
        }

        public async Task UpdateProduct(Product Product){
            _context.Products.Update(Product);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProduct(Product Product){
            _context.Products.Remove(Product);
            await _context.SaveChangesAsync();
        }
    }
}