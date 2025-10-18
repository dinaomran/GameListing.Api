using AutoMapper;
using GameListing.Api.Domain;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GameListing.Api.Common.Results;
using GameListing.Api.Common.Constants;
using GameListing.Api.Application.Contracts;
using GameListing.Api.Application.DTOs.Country;

namespace GameListing.Api.Application.Services;

public class CountriesService(GameListingDbContext context, IMapper mapper) : ICountriesService
{
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        try
        {
            //var countries = await context.Countries
            //    .Select(c => new GetCountriesDto(
            //        c.Id, c.Name
            //    ))
            //    .ToListAsync();

            var countries = await context.Countries
                .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            //return countries;

            return Result<IEnumerable<GetCountriesDto>>.Success(countries);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<GetCountriesDto>>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        try
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

            //return country ?? null;

            return country == null ? Result<GetCountryDto>.NotFound(new Error(ErrorCodes.NotFound, $"Country with ID {id} not found.")) : Result<GetCountryDto>.Success(country);
        }
        catch (Exception ex)
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        try
        {
            if (id != countryDto.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, $"The ID in the URL ({id}) does not match the ID in the body ({countryDto.Id})."));
            }

            //var country = await context.Countries.FindAsync(id) ?? throw new KeyNotFoundException($"Country with ID {id} not found.");

            var country = await context.Countries.FindAsync(id);

            if (country == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with ID {id} not found."));
            }

            var countryDuplicateName = await CountryExistsAsync(countryDto.Name);

            if (countryDuplicateName == true)
            {
                return Result.Failure(new Error(ErrorCodes.Conflict, $"Country with Name {countryDto.Name} already exists."));
            }

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
                    return Result.NotFound(new Error(ErrorCodes.NotFound, $"The following Player Ids do not exist: {string.Join(", ", missingIds)}"));
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
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto)
    {
        try
        {
            var countryExist = await CountryExistsAsync(countryDto.Name);

            if (countryExist == true)
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, $"Country with Name {countryDto.Name} already exists."));
            }

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
                    return Result<GetCountryDto>.NotFound(new Error(ErrorCodes.NotFound, $"The following Player Ids do not exist: {string.Join(", ", missingIds)}"));
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

            return Result<GetCountryDto>.Success(mapper.Map<GetCountryDto>(country));
        }
        catch (Exception ex)
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }
    
    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await context.Countries.FindAsync(id);

            if (country == null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country with ID {id} not found."));
            }

            context.Countries.Remove(country);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(ErrorCodes.Failure, ex.Message));
        }
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> CountryExistsAsync(string name)
    {
        return await context.Countries.AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
    }
}