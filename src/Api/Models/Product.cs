using ECommerce.Api.Enums;

namespace ECommerce.Api.Models {
    public class Product : AbstractEntity {
        public required string Name { get; set; }
        public ProductType ProductType{ get; set; } = ProductType.UNDEFINED;
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
    }
}
