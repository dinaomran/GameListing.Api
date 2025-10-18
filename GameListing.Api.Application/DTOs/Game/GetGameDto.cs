using GameListing.Api.Application.DTOs.Player;

namespace GameListing.Api.Application.DTOs.Game;

public record GetGameDto(
    int Id,
    string Title,
    string Category,
    DateOnly ReleaseDate,
    double Price,
    List<GetPlayersDto> Players
);

public record GetGamesDto(
    int Id,
    string Title,
    string Category,
    DateOnly ReleaseDate,
    double Price
);
