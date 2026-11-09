using AutoMapper;
using CommandService.DTOs;
using CommandService.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId, CancellationToken ct = default)
        {
            Console.WriteLine($"--> Hitted GetCommandForPlatform endpoint in CommandService. Id: {platformId}");

            if (!await _commandRepository.PlatformExists(platformId, ct))
            {
                return NotFound();
            }

            var commands = await _commandRepository.GetCommandsForPlatform(platformId, ct);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }


        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform: {platformId} / {commandId}");

            if (!await _commandRepository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = _commandRepository.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }
    }
}
