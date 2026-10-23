using PlatformService.Models;

namespace PlatformService.Interfaces
{
    public interface IPlatformRepo
    {
        public bool SaveChanges();
        public IEnumerable<Platform> GetAllPlatforms();
        public Platform? GetPlatformById(int id);
        public void CreatePlatform(Platform platform);
    }
}
