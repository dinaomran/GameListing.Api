using AutoMapper;
using GameListing.Api.Application.DTOs.Game;
using GameListing.Api.Domain;

namespace GameListing.Api.Application.MappingProfiles;

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