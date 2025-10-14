using Microsoft.AspNetCore.Mvc;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Game;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(IGamesService gamesService) : BaseApiController
{
    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetGamesDto>>> GetGames()
    {
        var result = await gamesService.GetGamesAsync();
        return ToActionResult(result);
    }

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetGameDto>> GetGame(int id)
    {
        var result = await gamesService.GetGameAsync(id);
        return ToActionResult(result);
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, UpdateGameDto gameDto)
    {
        var result = await gamesService.UpdateGameAsync(id, gameDto);
        return ToActionResult(result);
    }

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GetGameDto>> PostGame(CreateGameDto gameDto)
    {
        var result = await gamesService.CreateGameAsync(gameDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction(nameof(GetGame), new { id = result.Value!.Id }, result.Value);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var result = await gamesService.DeleteGameAsync(id);
        return ToActionResult(result);
    }
}
