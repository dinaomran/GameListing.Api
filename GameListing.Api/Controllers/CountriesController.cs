using GameListing.Api.Data;
using GameListing.Api.DTOs.Country;
using GameListing.Api.DTOs.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(GameListingDbContext context) : ControllerBase
{

    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = await context.Countries
            .Select(c => new GetCountriesDto(c.Id, c.Name))
            .ToListAsync();

        return Ok(countries);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
       //// This code will generate error because we need to do filter before doing select
       //var country = await context.Countries
       //    .Include(c => c.Players) // After using DTOs we now don't need to include related entities unless they are part of the DTO
       //    .Select(c => new GetCountryDto(c.Id, c.Name))
       //    .FirstOrDefaultAsync(q => q.Id == id);

        var country = await context.Countries
            .Where(c => c.Id == id) // Filter first
            .Select(c => new GetCountryDto(
                c.Id,
                c.Name,
                c.Players.Select(p => new GetPlayersDto(
                    p.Id,
                    p.Username,
                    p.Email,
                    p.CountryId
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        if (country == null)
        {
            return NotFound();
        }

        return country;
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto countryDto )
    {
        var country = await context.Countries.FindAsync(id);

        if (country == null)
        {
            return NotFound();
        }

        if (id != countryDto.Id)
        {
            return BadRequest(new { message = $"The ID in the URL ({id}) does not match the ID in the body ({countryDto.Id})." });
        }

        // After using Dto, Update fields because i don't know which fields were changed
        country.Name = countryDto.Name;

        if (countryDto.PlayerIds != null && countryDto.PlayerIds.Count != 0)
        {
            var playersExistIds = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = countryDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                return BadRequest($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
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

        context.Entry(country).State = EntityState.Modified; // After Using DTOs we need to update fields first then update entity state

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CountryExistsAsync(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto countryDto)
    {
        var country = new Country
        {
            Name = countryDto.Name
        };

        if (countryDto.PlayerIds != null && countryDto.PlayerIds.Count > 0)
        {
            var playersExistIds = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = countryDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                return BadRequest($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var players = await context.Players
                .Where(p => countryDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            country.Players = players;
        }

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.Id == id);
    }
}
