using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Data;

public class ECommerceDbContext : DbContext {
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; } 
    public DbSet<Customer> Customers { get; set; } 
}

