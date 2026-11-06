using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDB
    {
        public static async Task PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                await Seed(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static async Task Seed(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("Starting migrations:");
                try
                {
                    var connection = context.Database.GetDbConnection();

                    Console.WriteLine($"DB Server: {connection.DataSource}");
                    Console.WriteLine($"DB Name: {connection.Database}");
                    Console.WriteLine($"DB User: {connection.ConnectionString
                        .Split(';')
                        .FirstOrDefault(x => x.TrimStart().StartsWith("User Id=", StringComparison.OrdinalIgnoreCase))}");

                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while migration: {ex}");
                }
            }

            if (!await context.Platforms.AnyAsync())
            {
                Console.WriteLine("Seeding");

                context.Platforms.AddRange(
                    new Platform("Dot Net", "Microsoft", "Free"),
                    new Platform("SQL Server Express", "Microsoft", "Free"),
                    new Platform("Kubernetes", "Cloud Native Computing Foundation", "Free")
                    );

                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Data allready exists");
            }
        }

    }
}
