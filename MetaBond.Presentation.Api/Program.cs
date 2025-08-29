using MetaBond.Infrastructure.Persistence;
using MetaBond.Infrastructure.Shared;
using MetaBond.Application;
using Serilog;
using MetaBond.Presentation.Api.Extensions;

try
{
    Log.Information("starting server");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.RateLimiting();
    builder.Services.AddException();

    // DI layer
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddShared(builder.Configuration);
    builder.Services.AddApplicationLayer();
    builder.Services.AddSwaggerExtension();
    builder.Services.AddVersioning();

    var app = builder.Build();

    app.ApplyMigrations();

    app.UseExceptionHandler(_ => { });

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UserSwaggerExtension();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex}");
    Log.Fatal("server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}