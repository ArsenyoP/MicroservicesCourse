using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Interfaces;
using PlatformService.Repositories;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//seed DB
PrepDB.PrepPopulation(app);

app.Run();
