using GameListing.Api.Application.DTOs.Game;

namespace GameListing.Api.Application.DTOs.Player;

public record GetPlayerDto
(
    int Id,
    string Username,
    string Email,
    string CountryName,
    List<GetGamesDto> Games
);

public record GetPlayersDto
(
    int Id,
    string Username,
    string Email,
    int? CountryId
);