
namespace ECommerce.Api.Dtos {
    public class CreateProductSaleDto {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CreateSaleDto {
        public Guid CustomerId { get; set; }
        public DateTime Date { get; set; }
        public List<CreateProductSaleDto> Products { get; set; } = new();
    }
}