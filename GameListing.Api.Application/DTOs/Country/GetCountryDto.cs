using GameListing.Api.Application.DTOs.Player;

namespace GameListing.Api.Application.DTOs.Country;

public record GetCountryDto(
    int Id,
    string Name,
    List<GetPlayersDto> Players
);

public record GetCountriesDto(
    int Id,
    string Name
);
