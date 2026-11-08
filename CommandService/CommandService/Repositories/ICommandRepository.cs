using CommandService.Models;

namespace CommandService.Repositories;

public interface ICommandRepository
{
    Task<bool> SaveChanges(CancellationToken ct = default);
    Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken ct = default);
    void CreatePlatform(Platform platform);
    Task<bool> PlatformExists(int platformId, CancellationToken ct = default);

    Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId, CancellationToken ct = default);
    Task<Command?> GetCommand(int platformId, int commandId, CancellationToken ct = default);
    void CreateCommand(int platformId, Command command);
}