using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Dtos;
using PlatformService.Interfaces;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private IPlatformRepo _platformRepo;
        private IMapper _mapper;
        private ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBus;

        public PlatformsController(
            IPlatformRepo platformRepo,
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBus)
        {
            _platformRepo = platformRepo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms(CancellationToken cancellationToken = default)
        {
            var platformItems = await _platformRepo.GetAllPlatformsAsync(cancellationToken);

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPLatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id, CancellationToken cancellationToken = default)
        {
            var platformItem = await _platformRepo.GetPlatformByIdAsync(id, cancellationToken);

            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }

        //{
        //  "name": "Kubernetes",
        //  "publisher": "Cloud Native Computing Foundation",
        //  "cost": "Free"
        //}

        //http://localhost:5161/api/platforms
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformDto)
        {
            var platformItem = _mapper.Map<Platform>(platformDto);

            if (platformItem == null)
            {
                return BadRequest();
            }

            await _platformRepo.CreatePlatformAsync(platformItem);
            await _platformRepo.SaveChangesAsync();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformItem);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not sent synchronously: {ex}");
            }


            //async send
            try
            {
                var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);
                platformPublishDto.Event = "Platform_Published";

                await _messageBus.PublishNewPlatform(platformPublishDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Exception while sending PlatformCreated event: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
