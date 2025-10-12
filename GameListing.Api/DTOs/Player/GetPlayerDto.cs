using GameListing.Api.DTOs.Game;

namespace GameListing.Api.DTOs.Player;

public record GetPlayerDto
(
    int Id,
    string Username,
    string Email,
    string Country,
    List<GetGamesDto> Games
);

public record GetPlayersDto
(
    int Id,
    string Username,
    string Email,
    int? CountryId
);