using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Presentation.Api.Extensions;

public static class Extension
{
    public static void UserSwaggerExtension(this IApplicationBuilder builder)
    {
        builder.UseSwagger();
        builder.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "MetaBond"); });
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MetaBondContext>();
        db.Database.Migrate();
    }
}