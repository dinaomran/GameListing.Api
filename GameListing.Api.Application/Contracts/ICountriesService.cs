// Contract == Interface

using GameListing.Api.Application.DTOs.Country;
using GameListing.Api.Common.Results;

namespace GameListing.Api.Application.Contracts
{
    public interface ICountriesService
    {
        Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
        Task<Result<GetCountryDto>> GetCountryAsync(int id);
        Task<Result> UpdateCountryAsync(int id, UpdateCountryDto countryDto);
        Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto);
        Task<Result> DeleteCountryAsync(int id);
        Task<bool> CountryExistsAsync(int id);
        Task<bool> CountryExistsAsync(string name);
    }
}