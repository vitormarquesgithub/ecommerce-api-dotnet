namespace ECommerce.Api.Models  {
    public class ProductSale : AbstractEntity  {
        public Guid SaleId { get; set; }
        public Sale? Sale { get; set; }       

        public Guid ProductId { get; set; }
        public Product? Products { get; set; }   
        
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
