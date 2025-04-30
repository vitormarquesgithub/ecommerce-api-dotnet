using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Repositories {
    public interface ISaleRepository {
        Task<Sale> AddSale(Sale sale);
        Task<Sale?> GetSaleById(Guid id);
        Task<IEnumerable<Sale>> GetAllSales();
        Task RemoveSale(Sale sale);
        Task<SalesAnalysis> AnalyzeSales(DateTime start, DateTime end);
    }
}
