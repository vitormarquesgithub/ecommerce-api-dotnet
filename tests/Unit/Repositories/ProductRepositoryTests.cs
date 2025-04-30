using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Api.Models;
using ECommerce.Api.Enums;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Tests.Unit.Repositories {
    public class ProductRepositoryTests : IDisposable {
        private readonly ECommerceDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests() {
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ECommerceDbContext(options);
            _repository = new ProductRepository(_context);
        }

        [Fact]
        public async Task AddProduct_AddsToDatabase() {
            var newProduct = new Product { 
                Name = "Mouse Gamer",
                Description = "DPI ajustável",
                Price = 249.90m,
                StockCount = 60,
                ProductType = ProductType.ELECTRONICS
            };

            var result = await _repository.AddProduct(newProduct);

            var saved = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(saved);
            Assert.Equal("Mouse Gamer", saved.Name);
        }

        [Fact]
        public async Task UpdateProduct_ModifiesExistingProduct() {
            var product = new Product { 
                Name = "Cadeira Gamer",
                Description = "Ergonômica",
                Price = 1599.00m,
                StockCount = 10,
                ProductType = ProductType.HOME
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            
            product.Name = "Cadeira Gamer Pro";

            await _repository.UpdateProduct(product);

            var updated = await _context.Products.FindAsync(product.Id);
            Assert.Equal("Cadeira Gamer Pro", updated!.Name);
        }

        public void Dispose() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}