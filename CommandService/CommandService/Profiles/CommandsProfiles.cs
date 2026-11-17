using AutoMapper;
using CommandService.DTOs;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class CommandsProfiles : Profile
    {
        public CommandsProfiles()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreatreDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishDto, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
        }
    }
}
