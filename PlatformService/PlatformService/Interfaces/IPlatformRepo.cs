using PlatformService.Models;
using System.Threading;

namespace PlatformService.Interfaces
{
    public interface IPlatformRepo
    {
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken cancellationToken = default);
        Task<Platform?> GetPlatformByIdAsync(int id, CancellationToken cancellationToken = default);
        Task CreatePlatformAsync(Platform platform, CancellationToken cancellationToken = default);
    }
}
