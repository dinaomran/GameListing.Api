using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.DTOs.Country;

namespace GameListing.Api.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountriesDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id));
        CreateMap<Country, GetCountryDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id));
        CreateMap<UpdateCountryDto, Country>();
        CreateMap<CreateCountryDto, Country>();
    }
}