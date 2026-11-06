using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                Seed(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static async Task Seed(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("Starting migrations:");
                try
                {
                    await context.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception while migration: {ex.Message}");
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

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("Data allready exists");
            }
        }

    }
}
