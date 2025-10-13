using GameListing.Api.DTOs.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameListing.Api.Contracts;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController(IPlayersService playersService) : ControllerBase
{
    // GET: api/Players
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetPlayersDto>>> GetPlayers()
    {
        var players = await playersService.GetPlayersAsync();

        return Ok(players);
    }

    // GET: api/Players/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPlayerDto>> GetPlayer(int id)
    {
        var player = await playersService.GetPlayerAsync(id);

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
        if (id != playerDto.Id)
        {
            return BadRequest(new { message = $"The ID in the URL ({id}) does not match the ID in the body ({playerDto.Id})." });
        }

        try
        {
            await playersService.UpdatePlayerAsync(id, playerDto);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await playersService.PlayerExistsAsync(id))
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
    public async Task<ActionResult<GetPlayerDto>> PostPlayer(CreatePlayerDto playerDto)
    {
        var player = await playersService.CreatePlayerAsync(playerDto);

        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
    }

    // DELETE: api/Players/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        await playersService.DeletePlayerAsync(id);

        return NoContent();
    }
}
