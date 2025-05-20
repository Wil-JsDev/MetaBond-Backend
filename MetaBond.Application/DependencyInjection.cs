using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MetaBond.Application.Behaviors;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Services;

namespace MetaBond.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddProblemDetails();

            #region Services
            services.AddScoped<IEmailConfirmationTokenService, EmailConfirmationTokenService>();
            #endregion
        }
    }
}
