using GameListing.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayersController(GameListingDbContext context) : ControllerBase
{

    // GET: api/Players
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
    {
        return await context.Players.ToListAsync();
    }

    // GET: api/Players/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Player>> GetPlayer(int id)
    {
        var player = await context.Players
            .Include(p => p.Country)
            .Include(p => p.Games)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (player == null)
        {
            return NotFound();
        }

        return player;
    }

    // PUT: api/Players/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlayer(int id, Player player)
    {
        if (id != player.Id)
        {
            return BadRequest();
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
    public async Task<ActionResult<Player>> PostPlayer(Player player)
    {
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
