using System.Text.Json.Serialization;

namespace ECommerce.Api.Models {
    public class Sale : AbstractEntity {
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }  
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    [JsonIgnore]
    public List<ProductSale> Products { get; set; } = new();  
    }
}
