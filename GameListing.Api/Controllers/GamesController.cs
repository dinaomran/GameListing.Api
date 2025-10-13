using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(IGamesService gamesService) : ControllerBase
{
    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetGamesDto>>> GetGames()
    {
        var games = await gamesService.GetGamesAsync();

        return Ok(games);
    }

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetGameDto>> GetGame(int id)
    {
        var game = await gamesService.GetGameAsync(id);

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
        if (id != gameDto.Id)
        {
            return BadRequest(new { message = $"The ID in the URL ({id}) does not match the ID in the body ({gameDto.Id})." });
        }

        try
        {
            await gamesService.UpdateGameAsync(id, gameDto);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await gamesService.GameExistsAsync(id))
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
    public async Task<ActionResult<GetGameDto>> PostGame(CreateGameDto gameDto)
    {
        var game = await gamesService.CreateGameAsync(gameDto);

        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        await gamesService.DeleteGameAsync(id);

        return NoContent();
    }
}
