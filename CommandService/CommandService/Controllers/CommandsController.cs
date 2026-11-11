using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;
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

        [HttpGet("commands")]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetAllCommands(CancellationToken ct = default)
        {
            var commandObjects = await _commandRepository.GetAllCommands(ct);

            var commandDtos = _mapper.Map<IEnumerable<CommandReadDto>>(commandObjects);

            return Ok(commandDtos);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsForPlatform(int platformId, CancellationToken ct = default)
        {
            Console.WriteLine($"--> Hitted GetCommandForPlatform endpoint in CommandService. Id: {platformId}");

            if (!await _commandRepository.PlatformExists(platformId, ct))
            {
                Console.WriteLine(await _commandRepository.PlatformExists(platformId, ct));
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

            var command = await _commandRepository.GetCommand(platformId, commandId);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, CommandCreatreDto createDto, CancellationToken ct = default)
        {
            Console.WriteLine($"--> Hitted CreateCommandForPlatform account: {createDto}, platform id: {platformId}");

            if (await _commandRepository.PlatformExists(platformId, ct))
            {
                return NotFound();
            }

            var command = _mapper.Map<Command>(createDto);

            _commandRepository.CreateCommand(platformId, command);
            await _commandRepository.SaveChangesAsync(ct);

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}
