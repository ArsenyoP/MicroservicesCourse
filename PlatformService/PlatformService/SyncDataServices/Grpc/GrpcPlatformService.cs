using AutoMapper;
using Grpc.Core;
using PlatformService.Interfaces;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepo platformRepo, IMapper mapper)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
        }

        public override async Task<platformResponse> GetAllPlatforms(GetAllRequests request, ServerCallContext context)
        {
            var response = new platformResponse();
            var platforms = await _platformRepo.GetAllPlatformsAsync(context.CancellationToken);

            foreach (var plat in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }

            return response;
        }
    }
}
