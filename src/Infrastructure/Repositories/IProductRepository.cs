using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Repositories {
    public interface IProductRepository {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(Guid id);
        Task<Product> AddProduct(Product Product);
        Task UpdateProduct(Product Product);
        Task RemoveProduct(Product Product);
    }
}