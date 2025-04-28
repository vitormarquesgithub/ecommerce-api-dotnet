using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Data;

public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers { get; set; } 
}

