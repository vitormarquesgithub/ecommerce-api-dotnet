using ECommerce.Api.Controllers;
using ECommerce.Api.Models;
using ECommerce.Api.Enums;
using ECommerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ECommerce.Tests.Unit.Controllers {
    public class ProductsControllerTests {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductsController _controller;

        public ProductsControllerTests() {
            _mockRepo = new Mock<IProductRepository>();
            _controller = new ProductsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkWithProducts() {
            var products = new List<Product> {
                new Product { 
                    Id = Guid.NewGuid(),
                    Name = "Smartphone X",
                    Description = "Último modelo",
                    Price = 2999.90m,
                    StockCount = 50,
                    ProductType = ProductType.ELECTRONICS
                }
            };

            _mockRepo.Setup(repo => repo.GetAllProducts()).ReturnsAsync(products);

            var result = await _controller.GetProducts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Single(returnedProducts);
        }

        [Fact]
        public async Task GetProductById_NotFound_ReturnsNotFound() {
            _mockRepo.Setup(repo => repo.GetProductById(It.IsAny<Guid>())).ReturnsAsync((Product?)null);

            var result = await _controller.GetProductById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddProduct_Valid_ReturnsCreatedAtRoute() {
            var newProd = new Product { 
                Name = "Fone Bluetooth",
                Description = "Cancelamento de ruído",
                Price = 599.90m,
                StockCount = 80,
                ProductType = ProductType.ELECTRONICS
            };
            _mockRepo.Setup(repo => repo.AddProduct(newProd)).ReturnsAsync(newProd);

            var result = await _controller.AddProduct(newProd);

            var createdAt = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetProductById", createdAt.RouteName);
        }

        [Fact]
        public async Task UpdateProduct_Valid_ReturnsNoContent() {
            var id = Guid.NewGuid();
            var existing = new Product { 
                Id = id,
                Name = "Produto Antigo",
                Description = "Descrição antiga",
                Price = 99.90m,
                StockCount = 20,
                ProductType = ProductType.HOME
            };

            var updated = new Product { 
                Id = id,
                Name = "Produto Atualizado",
                Description = "Nova descrição",
                Price = 119.90m,
                StockCount = 15,
                ProductType = ProductType.HOME
            };
            
            _mockRepo.Setup(repo => repo.GetProductById(id)).ReturnsAsync(existing);

            var result = await _controller.UpdateProduct(id, updated);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_Found_ReturnsNoContent() {
            var prod = new Product { 
                Id = Guid.NewGuid(),
                Name = "Produto para Deletar",
                Description = "Descrição",
                Price = 49.90m,
                StockCount = 5,
                ProductType = ProductType.OTHER
            };

            _mockRepo.Setup(repo => repo.GetProductById(prod.Id)).ReturnsAsync(prod);

            var result = await _controller.DeleteProduct(prod.Id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}