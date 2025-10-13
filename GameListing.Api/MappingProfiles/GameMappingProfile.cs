using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.DTOs.Game;

namespace GameListing.Api.MappingProfiles;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        CreateMap<Game, GetGamesDto>();
        CreateMap<Game, GetGameDto>();
        CreateMap<UpdateGameDto, Game>();
        CreateMap<CreateGameDto, Game>();
    }
}
