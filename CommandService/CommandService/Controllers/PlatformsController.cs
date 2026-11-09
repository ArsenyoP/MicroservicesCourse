using AutoMapper;
using CommandService.DTOs;
using CommandService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/[controller]")] //api/c/Platforms
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _repository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms(CancellationToken ct = default)
        {
            Console.WriteLine("--> Getting platforms from Command Service");

            var platformItems = await _repository.GetAllPlatformsAsync(ct);

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound POST for commands service");
            return Ok("Inbound test ok from platforms controller");
        }
    }
}
