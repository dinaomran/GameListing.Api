using AutoMapper;
using GameListing.Api.Application.DTOs.Player;
using GameListing.Api.Domain;

namespace GameListing.Api.Application.MappingProfiles;

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