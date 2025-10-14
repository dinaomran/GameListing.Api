using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.DTOs.Player;

namespace GameListing.Api.MappingProfiles;

public class PlayerMappingProfile : Profile
{
    public PlayerMappingProfile()
    {
        CreateMap<Player, GetPlayersDto>();
        CreateMap<Player, GetPlayerDto>()
            .ForMember(d => d.CountryName, opt => opt.MapFrom(s => s.Country != null ? s.Country.Name : string.Empty));
        CreateMap<UpdatePlayerDto, Player>();
        CreateMap<CreatePlayerDto, Player>();
    }
}