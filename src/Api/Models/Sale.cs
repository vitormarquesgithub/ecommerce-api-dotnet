namespace ECommerce.Api.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int CustumerId { get; set; }
        public required Custumer Custumer { get; set; } 
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public required List<ProductSale> Products { get; set; }
    }

    public class ProductSale
    {
        public int ProductId { get; set; }
        public required Product Products { get; set; }
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
