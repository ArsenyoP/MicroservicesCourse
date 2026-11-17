using CommandService.Models;

namespace CommandService.SyncDataServices.Grpc
{
    public interface IPlatformDataClient
    {
        Task<IEnumerable<Platform>> ReturnAllPlatforms();
    }
}
