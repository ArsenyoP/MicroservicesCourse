using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContext = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_configuration["CommandServiceURL"]}", httpContext);


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync request to Command Seevice was successful");
            }
            else
            {
                Console.WriteLine($"--> Sync request to Command Seevice was NOT successful code: {response.StatusCode}");
            }
        }
    }
}
