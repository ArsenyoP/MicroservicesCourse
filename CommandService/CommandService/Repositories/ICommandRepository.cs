using CommandService.Models;

namespace CommandService.Repositories;

public interface ICommandRepository
{
    Task<IEnumerable<Command>> GetAllCommands(CancellationToken ct = default);
    Task<bool> SaveChangesAsync(CancellationToken ct = default);
    Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken ct = default);
    void CreatePlatform(Platform platform);
    Task<bool> PlatformExists(int platformId, CancellationToken ct = default);
    Task<bool> ExternalPlatformExists(int externalPlatformId, CancellationToken ct = default);


    Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId, CancellationToken ct = default);
    Task<Command?> GetCommand(int platformId, int commandId, CancellationToken ct = default);
    void CreateCommand(int platformId, Command command);
}