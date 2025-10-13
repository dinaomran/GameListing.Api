using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.DTOs.Player;

namespace GameListing.Api.MappingProfiles;

public class PlayerMappingProfile : Profile
{
    public PlayerMappingProfile()
    {
        //CreateMap<Player, GetPlayerDto>()
        //    .ForMember(d => d.Country, cfg => cfg.MapFrom(s => s.Country!.Name));
        CreateMap<Player, GetPlayersDto>();
        CreateMap<Player, GetPlayerDto>()
            .ForMember(d => d.Country, cfg => cfg.MapFrom<CountryNameResolver>());
        CreateMap<UpdatePlayerDto, Player>();
        CreateMap<CreatePlayerDto, Player>();
    }
}

public class CountryNameResolver : IValueResolver<Player, GetPlayerDto, string>
{
    public string Resolve(Player source, GetPlayerDto destination, string destMember, ResolutionContext context)
    {
        return source.Country?.Name ?? string.Empty;   
    }
}