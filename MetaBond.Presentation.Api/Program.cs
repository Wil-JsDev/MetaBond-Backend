using MetaBond.Infrastructure.Persistence;
using MetaBond.Infrastructure.Shared;
using MetaBond.Application;
using Serilog;

Log.Logger = new LoggerConfiguration()
             .WriteTo.Console()
             .CreateLogger();

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
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddShared(builder.Configuration);
    builder.Services.AddApplicationLayer();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex,"server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}