namespace ECommerce.Api.Models {
    public abstract class AbstractEntity {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}