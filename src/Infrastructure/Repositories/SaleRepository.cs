using ECommerce.Api.Models;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories {
    public class SaleRepository : ISaleRepository {
        private readonly ECommerceDbContext _context;
        public SaleRepository(ECommerceDbContext context) => _context = context;

        public async Task<Sale> AddSale(Sale sale) {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return sale;
        }

        public async Task<Sale?> GetSaleById(Guid id)
            => await _context.Sales
                .Include(s => s.Products)
                    .ThenInclude(ps => ps.Products)
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == id);

        public async Task<IEnumerable<Sale>> GetAllSales()
            => await _context.Sales
                .Include(s => s.Products)
                    .ThenInclude(ps => ps.Products)
                .Include(s => s.Customer)
                .AsNoTracking()
                .ToListAsync();


        public async Task RemoveSale(Sale sale) {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task<SalesAnalysis> AnalyzeSales(DateTime start, DateTime end) {
            var sales = await _context.Sales
                .Include(s => s.Products)
                    .ThenInclude(ps => ps.Products)
                .Where(s => s.Date >= start && s.Date <= end)
                .ToListAsync();

            var totalCount   = sales.Count;
            var totalRevenue = sales.Sum(s => s.Total);

            var perProduct = sales
                .SelectMany(s => s.Products)
                .GroupBy(ps => ps.ProductId)
                .Select(g => new ProductRevenue {
                    ProductId   = g.Key,
                    ProductName = g.First().Products!.Name,
                    Revenue     = g.Sum(ps => ps.UnitPrice * ps.Amount)
                })
                .ToList();

            return new SalesAnalysis {
                TotalSales       = totalCount,
                TotalRevenue     = totalRevenue,
                RevenueByProduct = perProduct
            };
        }
    }
}