using Xunit;
using Moq;
using ECommerce.Api.Controllers;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Api.Dtos;
using ECommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ECommerce.Tests.Unit.Controllers {
    public class SalesControllerTests {
        private readonly Mock<ISaleRepository> _repoMock;
        private readonly SalesController _controller;

        public SalesControllerTests() {
            _repoMock = new Mock<ISaleRepository>();
            _controller = new SalesController(_repoMock.Object);
        }

        [Fact]
        public async Task GetSales_ReturnsAllSales() {
            var sales = new List<Sale> { new Sale { Id = Guid.NewGuid(), Products = new List<ProductSale>() } };
            _repoMock.Setup(r => r.GetAllSales()).ReturnsAsync(sales);

            var result = await _controller.GetSales();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(sales, okResult.Value);
        }

        [Fact]
        public async Task GetSale_WithValidId_ReturnsSale() {
            var saleId = Guid.NewGuid();
            var sale = new Sale { Id = saleId, Products = new List<ProductSale>() };
            _repoMock.Setup(r => r.GetSaleById(saleId)).ReturnsAsync(sale);

            var result = await _controller.GetSale(saleId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(sale, okResult.Value);
        }

        [Fact]
        public async Task GetSale_WithInvalidId_ReturnsNotFound() {
            _repoMock.Setup(r => r.GetSaleById(It.IsAny<Guid>())).ReturnsAsync((Sale?)null);

            var result = await _controller.GetSale(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateSale_WithInvalidDto_ReturnsBadRequest() {
            var result = await _controller.CreateSale(null!);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateSale_WithValidDto_CreatesSale() {
            var dto = new CreateSaleDto {
                CustomerId = Guid.NewGuid(),
                Date = DateTime.Now,
                Products = new List<CreateProductSaleDto> {
                    new CreateProductSaleDto { ProductId = Guid.NewGuid(), Amount = 2, UnitPrice = 10 }
                }
            };

            _repoMock.Setup(r => r.AddSale(It.IsAny<Sale>()))
                     .ReturnsAsync((Sale s) => { 
                         s.Id = Guid.NewGuid(); 
                         s.Products = new List<ProductSale>();
                         return s; 
                     });

            var result = await _controller.CreateSale(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(created.Value);
        }

        [Fact]
        public async Task DeleteSale_WithNonexistentId_ReturnsNotFound() {
            _repoMock.Setup(r => r.GetSaleById(It.IsAny<Guid>())).ReturnsAsync((Sale?)null);

            var result = await _controller.DeleteSale(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteSale_WithExistingId_ReturnsNoContent() {
            var sale = new Sale { Id = Guid.NewGuid(), Products = new List<ProductSale>() };
            _repoMock.Setup(r => r.GetSaleById(sale.Id)).ReturnsAsync(sale);

            var result = await _controller.DeleteSale(sale.Id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}