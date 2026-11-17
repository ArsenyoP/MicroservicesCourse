using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Platform>> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling gRPC service: {_configuration["GrpcPlatform"]}");

            var chanel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]!);
            var client = new GrpcPlatform.GrpcPlatformClient(chanel);
            var request = new GetAllRequests();

            try
            {
                var reply = await client.GetAllPlatformsAsync(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception was thrown while calling gRPC ReturnAllPlatforms: {ex.Message}");
                return null;
            }
        }
    }
}
