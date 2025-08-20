using Asp.Versioning;
using MetaBond.Presentation.Api.ExceptionHandler;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;

namespace MetaBond.Presentation.Api.Extensions;

public static class ServiceExtension
{
    public static void AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "MetaBond Api",
                Description = "",
                Contact = new OpenApiContact
                {
                    Name = "Wilmer De La Cruz"
                }
            });
            option.EnableAnnotations();
        });
    }

    public static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified =
                true; //When no versions are sent, this assumes the default version which is V1
            options.ReportApiVersions = true;
        });
    }

    public static void RateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, toke) =>
            {
                await context.HttpContext.Response.WriteAsync("Request limit exceeded. Please try again later");
            };

            options.AddFixedWindowLimiter("fixed", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.PermitLimit = 8;
            });

            options.AddSlidingWindowLimiter("sliding", limiterOptions =>
            {
                limiterOptions.PermitLimit = 5;
                limiterOptions.SegmentsPerWindow = 5;
                limiterOptions.Window = TimeSpan.FromMinutes(4);
            });
        });
    }

    public static void AddException(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}