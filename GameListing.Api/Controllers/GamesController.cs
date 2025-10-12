using GameListing.Api.Data;
using GameListing.Api.DTOs.Game;
using GameListing.Api.DTOs.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Linq;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(GameListingDbContext context) : ControllerBase
{

    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetGamesDto>>> GetGames()
    {
        var games = await context.Games
            .Select(g => new GetGamesDto(g.Id, g.Title, g.Category, g.ReleaseDate, g.Price))
            .ToListAsync();

        return Ok(games);
    }

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetGameDto>> GetGame(int id)
    {
        var game = await context.Games
            .Where(g => g.Id == id)
            .Select(g => new GetGameDto(
                g.Id,
                g.Title,
                g.Category,
                g.ReleaseDate,
                g.Price,
                g.Players.Select(p => new GetPlayersDto(
                    p.Id,
                    p.Username,
                    p.Email,
                    p.CountryId
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        if (game == null)
        {
            return NotFound();
        }

        return game;
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, UpdateGameDto gameDto)
    {
        var game = await context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        if (id != gameDto.Id)
        {
            return BadRequest(new { message = $"The ID in the URL ({id}) does not match the ID in the body ({gameDto.Id})." });
        }

        game.Title = gameDto.Title;
        game.Category = gameDto.Category;
        game.ReleaseDate = gameDto.ReleaseDate;
        game.Price = gameDto.Price;

        if (gameDto.PlayerIds != null && gameDto.PlayerIds.Count > 0)
        {
            var playersExistIds = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = gameDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                return BadRequest($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            // First Remove relationship for all players with game
            game.Players.Clear();
            await context.SaveChangesAsync(); // <— force deletion of join rows first

            var players = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            game.Players = players;
        }

        context.Entry(game).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await GameExistsAsync(id))
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

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> PostGame(CreateGameDto gameDto)
    {
        var game = new Game
        {
            Title = gameDto.Title,
            Category = gameDto.Category,
            ReleaseDate = gameDto.ReleaseDate,
            Price = gameDto.Price
        };

        if (gameDto.PlayerIds != null && gameDto.PlayerIds.Count > 0)
        {
            var playersExistIds = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var missingIds = gameDto.PlayerIds.Except(playersExistIds).ToList();

            if (missingIds.Count != 0)
            {
                return BadRequest($"The following Player Ids do not exist: {string.Join(", ", missingIds)}");
            }

            var players = await context.Players
                .Where(p => gameDto.PlayerIds.Contains(p.Id))
                .ToListAsync();

            game.Players = players;
        }

        context.Games.Add(game);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetGame", new { id = game.Id }, game);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var game = await context.Games.FindAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        context.Games.Remove(game);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> GameExistsAsync(int id)
    {
        return await context.Games.AnyAsync(e => e.Id == id);
    }
}
