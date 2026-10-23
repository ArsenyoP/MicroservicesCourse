using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDB
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                Seed(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void Seed(AppDbContext context)
        {
            if (!context.Platforms.Any())
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
