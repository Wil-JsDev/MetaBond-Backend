using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MetaBond.Infrastructure.Persistence.Context
{
    public class MetaBondContextFactory : IDesignTimeDbContextFactory<MetaBondContext>
    {
        public MetaBondContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("MetaBondBackend");

            var optionsBuilder = new DbContextOptionsBuilder<MetaBondContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new MetaBondContext(optionsBuilder.Options);
        }
    }
}