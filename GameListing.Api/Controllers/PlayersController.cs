using Microsoft.AspNetCore.Mvc;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Player;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController(IPlayersService playersService) : BaseApiController
{
    // GET: api/Players
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetPlayersDto>>> GetPlayers()
    {
        var result = await playersService.GetPlayersAsync();
        return ToActionResult(result);
    }

    // GET: api/Players/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPlayerDto>> GetPlayer(int id)
    {
        var result = await playersService.GetPlayerAsync(id);
        return ToActionResult(result);
    }

    // PUT: api/Players/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlayer(int id, UpdatePlayerDto playerDto)
    {
        var result = await playersService.UpdatePlayerAsync(id, playerDto);
        return ToActionResult(result);
    }

    // POST: api/Players
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GetPlayerDto>> PostPlayer(CreatePlayerDto playerDto)
    {
        var result = await playersService.CreatePlayerAsync(playerDto);
        if (!result.IsSuccess) return MapErrorsToResponse(result.Errors);

        return CreatedAtAction(nameof(GetPlayer), new { id = result.Value!.Id }, result.Value);
    }

    // DELETE: api/Players/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        var result = await playersService.DeletePlayerAsync(id);
        return ToActionResult(result);
    }
}