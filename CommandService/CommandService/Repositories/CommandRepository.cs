using CommandService.Data;
using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Repositories
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _dbContext;

        public CommandRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;

            _dbContext.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform is null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _dbContext.Platforms.Add(platform);
        }

        public async Task<IEnumerable<Command>> GetAllCommands(CancellationToken ct = default)
        {
            return await _dbContext.Commands.ToListAsync(ct);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken ct = default)
        {
            return await _dbContext.Platforms.ToListAsync(ct);
        }

        public async Task<Command?> GetCommand(int platformId, int commandId, CancellationToken ct = default)
        {
            return await _dbContext.Commands
                .Where(c => c.Id == commandId && c.PlatformId == platformId)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IEnumerable<Command>> GetCommandsForPlatform(int platformId, CancellationToken ct = default)
        {
            return await _dbContext.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(p => p.Platform.Name)
                .ToListAsync(ct);
        }

        public async Task<bool> PlatformExists(int platformId, CancellationToken ct = default)
        {
            return await _dbContext.Platforms.AnyAsync(p => p.Id == platformId);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _dbContext.SaveChangesAsync(ct) >= 0;
        }
    }
}
