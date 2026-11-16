using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Interfaces;
using PlatformService.Models;
using System.Threading;

namespace PlatformService.Repositories
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreatePlatformAsync(Platform platform, CancellationToken cancellationToken = default)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            await _context.Platforms.AddAsync(platform, cancellationToken);
        }

        public async Task<IEnumerable<Platform>> GetAllPlatformsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Platforms.ToListAsync(cancellationToken);
        }

        public async Task<Platform?> GetPlatformByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Platforms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return (await _context.SaveChangesAsync(cancellationToken) >= 0);
        }
    }
}
