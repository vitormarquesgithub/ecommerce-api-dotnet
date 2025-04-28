using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ECommerce.Infrastructure.Data {
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext> {
        public ECommerceDbContext CreateDbContext(string[] args) {
            var infrastructureDir = Directory.GetCurrentDirectory();
            var repoRoot       = Path.GetFullPath(Path.Combine(infrastructureDir, "..", ".."));
            var apiConfigDir   = Path.Combine(repoRoot, "src", "Api", "Properties");

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "src", "Api", "Properties")) // Caminho correto
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var connString = config.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("ConnectionString não encontrada");

            var optionsBuilder = new DbContextOptionsBuilder<ECommerceDbContext>();
            optionsBuilder.UseSqlServer(connString);

            return new ECommerceDbContext(optionsBuilder.Options);
        }
    }
}
