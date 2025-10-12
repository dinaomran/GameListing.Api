using GameListing.Api.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesTestController : ControllerBase
{
    private static List<Game> games = new List<Game>
    {
        new() { Id = 1, Title = "Call of Duty: Black Ops 6", Category = "FPS", ReleaseDate = new DateOnly(2024, 10, 25), Price = 69.99 },
        new() { Id = 2, Title = "God of War", Category = "Action-adventure", ReleaseDate = new DateOnly(2022, 11, 9), Price = 39.99},
        new() { Id = 3, Title = "Grand Theft Auto V", Category = "Action-adventure", ReleaseDate = new DateOnly(2013, 9, 17), Price = 29.99}
    };

    // GET: api/<GamesController>
    [HttpGet]
    public ActionResult<IEnumerable<Game>> Get()
    {
        return Ok(games);
    }

    // GET api/<GamesController>/5
    [HttpGet("{id}")]
    public ActionResult<Game> Get(int id)
    {
        var game = games.FirstOrDefault(g => g.Id == id);
        return game == null ? NotFound() : Ok(game);
    }

    // POST api/<GamesController>
    [HttpPost]
    public ActionResult<Game> Post([FromBody] Game newGame)
    {
        if (games.Any(g => g.Id == newGame.Id))
        {
            return BadRequest($"A game with Id {newGame.Id} already exists.");
        }

        games.Add(newGame);
        return CreatedAtAction(nameof(Get), new { id = newGame.Id }, newGame);
    }

    // PUT api/<GamesController>/5
    [HttpPut("{id}")]
    public ActionResult Put(int id, [FromBody] Game updatedGame)
    {
        var game = games.FirstOrDefault(g => g.Id == id);
        if (game == null)
        {
            return NotFound();
        }

        game.Title = updatedGame.Title;
        game.Category = updatedGame.Category;
        game.ReleaseDate = updatedGame.ReleaseDate;
        game.Price = updatedGame.Price;

        return NoContent();
    }

    // DELETE api/<GamesController>/5
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var game = games.FirstOrDefault(g => g.Id == id);
        return game == null ? NotFound(new { message = "Game not found" }) : NoContent();
    }
}
