namespace ECommerce.Api.Models
{
    public class Custumer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Telephone { get; set; }
    }
}
