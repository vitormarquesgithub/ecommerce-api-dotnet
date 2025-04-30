using ECommerce.Api.Models;
using ECommerce.Api.Enums;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Tests.Unit.Repositories
{
    public class SaleRepositoryTests : IDisposable
    {
        private readonly ECommerceDbContext _context;
        private readonly SaleRepository _repository;

        public SaleRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ECommerceDbContext(options);
            _repository = new SaleRepository(_context);
            
            SeedTestData();
        }

        private void SeedTestData()
        {
            var customer = new Customer 
            { 
                Username = "testuser",
                Password = "hashed",
                Name = "Test User",
                Email = "test@example.com",
                Telephone = "1234567890"
            };
            _context.Customers.Add(customer);

            var products = new List<Product>
            {
                new Product { 
                    Id = Guid.NewGuid(),
                    Name = "Product A", 
                    Description = "Description A",
                    Price = 100.00m, 
                    StockCount = 10,
                    ProductType = ProductType.ELECTRONICS
                },
                new Product { 
                    Id = Guid.NewGuid(),
                    Name = "Product B", 
                    Description = "Description B",
                    Price = 200.00m, 
                    StockCount = 5,
                    ProductType = ProductType.ELECTRONICS
                }
            };
            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        private Sale CreateTestSale()
        {
            var customer = _context.Customers.First();
            var products = _context.Products.ToList();

            return new Sale
            {
                CustomerId = customer.Id,
                Date = DateTime.UtcNow,
                Total = products[0].Price * 2 + products[1].Price * 1,
                Products = new List<ProductSale>
                {
                    new ProductSale { 
                        ProductId = products[0].Id, 
                        Amount = 2, 
                        UnitPrice = products[0].Price,
                        Products = products[0]
                    },
                    new ProductSale { 
                        ProductId = products[1].Id, 
                        Amount = 1, 
                        UnitPrice = products[1].Price,
                        Products = products[1]
                    }
                }
            };
        }

        [Fact]
        public async Task AddSale_ShouldAddSaleWithProducts()
        {
            var sale = CreateTestSale();
            var result = await _repository.AddSale(sale);
            
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(2, result.Products.Count);
            Assert.Equal(400.00m, result.Total);
        }

        [Fact]
        public async Task GetSaleById_ShouldReturnSaleWithRelationships()
        {
            var sale = CreateTestSale();
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            var result = await _repository.GetSaleById(sale.Id);

            Assert.NotNull(result);
            Assert.NotNull(result!.Customer);
            Assert.Equal(2, result.Products.Count);
            Assert.NotNull(result.Products[0].Products);
            Assert.Equal("Product A", result.Products[0].Products!.Name);
        }

        [Fact]
        public async Task GetAllSales_ShouldReturnAllSalesWithRelationships()
        {
            _context.Sales.Add(CreateTestSale());
            _context.Sales.Add(CreateTestSale());
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllSales();

            Assert.Equal(2, result.Count());
            Assert.All(result, s => Assert.NotNull(s!.Customer));
            Assert.All(result, s => Assert.NotEmpty(s!.Products));
        }

        [Fact]
        public async Task RemoveSale_ShouldDeleteSale()
        {
            var sale = CreateTestSale();
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            await _repository.RemoveSale(sale);
            var result = await _context.Sales.FindAsync(sale.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task AnalyzeSales_ShouldReturnCorrectAnalysis()
        {
            var startDate = DateTime.UtcNow.AddDays(-1);
            var endDate = DateTime.UtcNow.AddDays(1);
            
            var sale1 = CreateTestSale();
            
            var sale2 = CreateTestSale();
            sale2.Products[0].Amount = 3;
            sale2.Products[1].Amount = 2;
            sale2.Total = sale2.Products[0].UnitPrice * 3 + sale2.Products[1].UnitPrice * 2;
            
            _context.Sales.Add(sale1);
            _context.Sales.Add(sale2);
            await _context.SaveChangesAsync();

            var result = await _repository.AnalyzeSales(startDate, endDate);

            Assert.Equal(2, result.TotalSales);
            Assert.Equal(1100.00m, result.TotalRevenue);
            
            var productA = result.RevenueByProduct.First(p => p.ProductName == "Product A");
            var productB = result.RevenueByProduct.First(p => p.ProductName == "Product B");
            
            Assert.Equal(500.00m, productA.Revenue);
            Assert.Equal(600.00m, productB.Revenue);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}