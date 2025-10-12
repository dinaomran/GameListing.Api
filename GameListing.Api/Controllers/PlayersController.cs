using GameListing.Api.Data;
using GameListing.Api.DTOs.Player;
using GameListing.Api.DTOs.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController(GameListingDbContext context) : ControllerBase
{

    // GET: api/Players
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetPlayersDto>>> GetPlayers()
    {
        var players = await context.Players
            .Select(p => new GetPlayersDto(p.Id, p.Username, p.Email, p.CountryId))
            .ToListAsync();

        return Ok(players);
    }

    // GET: api/Players/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPlayerDto>> GetPlayer(int id)
    {
        var player = await context.Players
            .Include(p => p.Country)
            .Include(p => p.Games)
            .Where(p => p.Id == id)
            .Select(p => new GetPlayerDto(
                p.Id,
                p.Username,
                p.Email,
                p.Country!.Name,
                p.Games.Select(g => new GetGamesDto(
                    g.Id,
                    g.Title,
                    g.Category,
                    g.ReleaseDate,
                    g.Price
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        if (player == null)
        {
            return NotFound();
        }

        return player;
    }

    // PUT: api/Players/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlayer(int id, UpdatePlayerDto playerDto)
    {
        var player = await context.Players.FindAsync(id);

        if (player == null)
        {
            return NotFound();
        }

        if (id != playerDto.Id)
        {
            return BadRequest(new { message = $"The ID in the URL ({id}) does not match the ID in the body ({playerDto.Id})." });
        }

        player.Username = playerDto.Username;
        player.Email = playerDto.Email;
        player.CountryId = playerDto.CountryId;

        if (playerDto.GameIds != null && playerDto.GameIds.Count > 0)
        {
            var gamesExistIds = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            var missingIds = playerDto.GameIds.Except(gamesExistIds).ToList();

            if (missingIds.Count != 0)
            {
                return BadRequest($"The following Game Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var games = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .ToListAsync();

            player.Games = games;
        }

        context.Entry(player).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await PlayerExistsAsync(id))
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

    // POST: api/Players
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Player>> PostPlayer(CreatePlayerDto playerDto)
    {
        if (playerDto.CountryId != 0)
        {
            var countryExists = await context.Countries
                .AnyAsync(c => c.Id == playerDto.CountryId);

            if (!countryExists)
            {
                return BadRequest($"Country with Id {playerDto.CountryId} does not exist.");
            }
        }

        var player = new Player
        {
            Username = playerDto.Username,
            Email = playerDto.Email,
            CountryId = playerDto.CountryId
        };

        if (playerDto.GameIds != null && playerDto.GameIds.Count > 0)
        {
            var gamesExistIds = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync();

            var missingIds = playerDto.GameIds.Except(gamesExistIds).ToList();

            if (missingIds.Count != 0)
            {
                return BadRequest($"The following Game Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var games = await context.Games
                .Where(g => playerDto.GameIds.Contains(g.Id))
                .ToListAsync();

            player.Games = games;
        }

        context.Players.Add(player);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetPlayer", new { id = player.Id }, player);
    }

    // DELETE: api/Players/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        var player = await context.Players.FindAsync(id);
        if (player == null)
        {
            return NotFound();
        }

        context.Players.Remove(player);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> PlayerExistsAsync(int id)
    {
        return await context.Players.AnyAsync(e => e.Id == id);
    }
}
