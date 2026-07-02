using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace KFH
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<KFHContext>
    {
        public KFHContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<KFHContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new KFHContext(optionsBuilder.Options);
        }
    }
}
