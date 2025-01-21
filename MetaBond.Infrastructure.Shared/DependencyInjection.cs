using MetaBond.Application.Interfaces.Service;
using MetaBond.Domain.Settings;
using MetaBond.Infrastructure.Shared.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MetaBond.Infrastructure.Shared
{
    public static class DependencyInjection
    {
        public static void AddShared(this IServiceCollection services, IConfiguration configuration)
        {
            #region Configuration
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            #endregion

            #region Service
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            #endregion
        }
    }
}
