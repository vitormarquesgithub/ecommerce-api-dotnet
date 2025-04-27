namespace ECommerce.Domain.Entities;

public class Product
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = default!;
    public decimal Price      { get; set; }
    public int    StockCount  { get; set; }
}
