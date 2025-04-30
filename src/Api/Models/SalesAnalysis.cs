namespace ECommerce.Api.Models {
    public class SalesAnalysis {
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ProductRevenue> RevenueByProduct { get; set; } = new();
    }

    public class ProductRevenue {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }
}