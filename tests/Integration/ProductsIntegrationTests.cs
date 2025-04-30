using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Api.Controllers;
using ECommerce.Api.Models;
using ECommerce.Api.Enums;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Tests.Integration {
    public class ProductsIntegrationTests : IDisposable {
        private readonly ECommerceDbContext _context;
        private readonly ProductRepository _repository;
        private readonly ProductsController _controller;

        public ProductsIntegrationTests() {
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new ECommerceDbContext(options);
            _repository = new ProductRepository(_context);
            _controller = new ProductsController(_repository);
            
            _context.Products.AddRange(
                new Product { 
                    Name = "Monitor 24\"", 
                    Description = "Full HD", 
                    Price = 1299.90m, 
                    StockCount = 30,
                    ProductType = ProductType.ELECTRONICS
                },
                new Product { 
                    Name = "Mesa Escritório", 
                    Description = "Ajustável", 
                    Price = 899.00m, 
                    StockCount = 15,
                    ProductType = ProductType.HOME
                });
            _context.SaveChanges();
        }

        [Fact]
        public async Task FullCRUD_Workflow() {
            var newProduct = new Product { 
                Name = "Teclado Mecânico",
                Description = "Switches Red",
                Price = 349.90m,
                StockCount = 40,
                ProductType = ProductType.ELECTRONICS
            };
            
            var createResult = await _controller.AddProduct(newProduct);
            var createdResult = Assert.IsType<CreatedAtRouteResult>(createResult.Result);
            var createdProduct = Assert.IsType<Product>(createdResult.Value);

            var getResult = await _controller.GetProductById(createdProduct.Id);
            var okResult = Assert.IsType<OkObjectResult>(getResult.Result);
            var fetchedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal("Teclado Mecânico", fetchedProduct.Name);

            fetchedProduct.Name = "Teclado Mecânico Pro";
            await _controller.UpdateProduct(fetchedProduct.Id, fetchedProduct);
     
            var updatedResult = await _controller.GetProductById(fetchedProduct.Id);
            var updatedOkResult = Assert.IsType<OkObjectResult>(updatedResult.Result);
            var updatedProduct = Assert.IsType<Product>(updatedOkResult.Value);
            Assert.Equal("Teclado Mecânico Pro", updatedProduct.Name);

            await _controller.DeleteProduct(updatedProduct.Id);
            
            var deletedResult = await _controller.GetProductById(updatedProduct.Id);
            Assert.IsType<NotFoundResult>(deletedResult.Result);
        }

        public void Dispose() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}