using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.DTOs.Country;

namespace GameListing.Api.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountriesDto>();
        CreateMap<Country, GetCountryDto>();
        CreateMap<UpdateCountryDto, Country>();
        CreateMap<CreateCountryDto, Country>();
    }
}