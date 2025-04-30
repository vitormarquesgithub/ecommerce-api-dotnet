using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Models;

namespace ECommerce.Infrastructure.Data;

public class ECommerceDbContext : DbContext {
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; } 
    public DbSet<Customer> Customers { get; set; } 
    public DbSet<Sale> Sales { get; set; }
    public DbSet<ProductSale> ProductSales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductSale>()
            .HasOne(ps => ps.Sale)
            .WithMany(s => s.Products)
            .HasForeignKey(ps => ps.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductSale>()
            .HasOne(ps => ps.Products)
            .WithMany()
            .HasForeignKey(ps => ps.ProductId);
    }
}

