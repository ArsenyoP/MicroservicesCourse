using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;

namespace CommandService.Profiles
{
    public class CommandsProfiles : Profile
    {
        public CommandsProfiles()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreatreDto, Command>();
            CreateMap<Command, CommandReadDto>();
        }
    }
}
