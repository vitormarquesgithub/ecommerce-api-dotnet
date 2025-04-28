using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext>
    {
        public ECommerceDbContext CreateDbContext(string[] args)
        {
            // Diretório atual do projeto Infrastructure (onde este arquivo reside)
            var infrastructureDir = Directory.GetCurrentDirectory();
            // Subir dois níveis até a raiz do repositório
            var repoRoot       = Path.GetFullPath(Path.Combine(infrastructureDir, "..", ".."));
            // Unidade de configuração na pasta onde estão os appsettings da API
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
