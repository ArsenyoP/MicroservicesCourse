using CommandService.Models;
using CommandService.Repositories;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static async Task PrepPopulation(IApplicationBuilder builder)
        {
            using (var serviceScope = builder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = await grpcClient!.ReturnAllPlatforms();

                if (platforms is null)
                {
                    Console.WriteLine("--> No platforms returned from gRPC; skipping seed");
                    return;
                }

                await SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>()!, platforms);
            }
        }

        private static async Task SeedData(ICommandRepository repo, IEnumerable<Platform> platforms, CancellationToken ct = default)
        {
            Console.WriteLine("Seeding new platforms through gRPC");

            foreach (var plat in platforms)
            {
                if (!await repo.ExternalPlatformExists(plat.Id, ct))
                {
                    repo.CreatePlatform(plat);
                }
            }
            await repo.SaveChangesAsync(ct);
        }
    }
}
