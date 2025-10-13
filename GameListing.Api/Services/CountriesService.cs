using AutoMapper;
using GameListing.Api.Data;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Country;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace GameListing.Api.Services;

public class CountriesService(GameListingDbContext context, IMapper mapper) : ICountriesService
{
    public async Task<IEnumerable<GetCountriesDto>> GetCountriesAsync()
    {
        //var countries = await context.Countries
        //    .Select(c => new GetCountriesDto(
        //        c.Id, c.Name
        //    ))
        //    .ToListAsync();

        var countries = await context.Countries
            .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return countries;
    }

    public async Task<GetCountryDto?> GetCountryAsync(int id)
    {
        //// This code will generate error because we need to do filter before doing select
        //var country = await context.Countries
        //    .Include(c => c.Players) // After using DTOs we now don't need to include related entities unless they are part of the DTO
        //    .Select(c => new GetCountryDto(c.Id, c.Name))
        //    .FirstOrDefaultAsync(q => q.Id == id);

        //var country = await context.Countries
        //    .Where(c => c.Id == id) // Filter first
        //    .Select(c => new GetCountryDto(
        //        c.Id,
        //        c.Name,
        //        c.Players.Select(p => new GetPlayersDto(
        //            p.Id,
        //            p.Username,
        //            p.Email,
        //            p.CountryId
        //        )).ToList()
        //    ))
        //    .FirstOrDefaultAsync();

        var country = await context.Countries
            .Where(c => c.Id == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return country ?? null;
    }

    public async Task UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        var country = await context.Countries.FindAsync(id) ?? throw new KeyNotFoundException($"Country with ID {id} not found.");

        // After using Dto, Update fields because i don't know which fields were changed
        //country.Name = countryDto.Name;

        mapper.Map(countryDto, country);

        if (countryDto.PlayerIds != null && countryDto.PlayerIds.Count != 0)
        {
            var playersExistIds = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = countryDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                throw new KeyNotFoundException($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            // First Remove relationship for all players with country
            var playersToBeRemoved = await context.Players
                .Where(p => p.CountryId == country.Id && !countryDto.PlayerIds.Contains(p.Id)) // Remove relationship from any player that NOT IN the update request body
                .ToListAsync();

            foreach (var player in playersToBeRemoved)
            {
                player.CountryId = null;
            }

            // Then add the new players
            var players = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            country.Players = players;
        }

        //context.Entry(country).State = EntityState.Modified; // After Using DTOs we need to update fields first then update entity state
        
        await context.SaveChangesAsync();
    }

    public async Task<GetCountryDto> CreateCountryAsync(CreateCountryDto countryDto)
    {
        //var country = new Country
        //{
        //    Name = countryDto.Name
        //};

        var country = mapper.Map<Country>(countryDto);

        if (countryDto.PlayerIds != null && countryDto.PlayerIds.Count > 0)
        {
            var playersExistIds = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = countryDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                throw new KeyNotFoundException($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var players = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            country.Players = players;
        }

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        //return new GetCountryDto(
        //    country.Id,
        //    country.Name,
        //    country.Players.Select(p => new GetPlayersDto(
        //        p.Id,
        //        p.Username,
        //        p.Email,
        //        p.CountryId
        //    )).ToList()
        //);

        return mapper.Map<GetCountryDto>(country);
    }
    
    public async Task DeleteCountryAsync(int id)
    {
        var country = await context.Countries.FindAsync(id) ?? throw new KeyNotFoundException("Country with ID {id} not found.");
        context.Countries.Remove(country);
        await context.SaveChangesAsync();
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.Id == id);
    }
}