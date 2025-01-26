using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MetaBond.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

        }
    }
}
