using GameListing.Api.DTOs.Player;

namespace GameListing.Api.DTOs.Country;

public record GetCountryDto(
    int Id,
    string Name,
    List<GetPlayersDto> Players
);

public record GetCountriesDto(
    int Id,
    string Name
);
