using GameListing.Api.DTOs.Player;

namespace GameListing.Api.DTOs.Game;

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
