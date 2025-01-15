using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Infrastructure.Persistence
{
    public static class DependecyInjection
    {

        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            #region DbContext
            services.AddDbContext<MetaBondContext>(postgres =>
            {
                postgres.UseNpgsql(configuration.GetConnectionString("MetaBondBackend"), b =>
                {
                    b.MigrationsAssembly("MetaBond.Infrastructure.Persistence");
                });
            });

            #endregion

        }
    }
}
