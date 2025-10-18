using AutoMapper;
using GameListing.Api.Application.DTOs.Country;
using GameListing.Api.Domain;

namespace GameListing.Api.Application.MappingProfiles;

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