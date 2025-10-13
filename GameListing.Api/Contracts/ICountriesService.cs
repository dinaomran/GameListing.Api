// Contract == Interface

using GameListing.Api.DTOs.Country;

namespace GameListing.Api.Contracts
{
    public interface ICountriesService
    {
        Task<IEnumerable<GetCountriesDto>> GetCountriesAsync();
        Task<GetCountryDto?> GetCountryAsync(int id);
        Task UpdateCountryAsync(int id, UpdateCountryDto countryDto);
        Task<GetCountryDto> CreateCountryAsync(CreateCountryDto countryDto);
        Task DeleteCountryAsync(int id);
        Task<bool> CountryExistsAsync(int id);
    }
}