using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Interfaces;
using PlatformService.Repositories;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();


if (builder.Environment.IsProduction())
{
    Console.WriteLine($"Connection string: {builder.Configuration.GetConnectionString("PlatformsConnection")}");

    Console.WriteLine($"--> Using SQL Server: is prod: {builder.Environment.IsProduction()}");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection")));
}
else
{
    Console.WriteLine($"--> Using In-Memory DB is prod: {builder.Environment.IsProduction()}");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}


builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//seed DB
await PrepDB.PrepPopulation(app, builder.Environment.IsProduction());

app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/platforms.proto"));
});

app.Run();
