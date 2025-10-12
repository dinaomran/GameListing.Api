namespace GameListing.Api.Data;

public class Game
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Category { get; set; }
    public DateOnly ReleaseDate { get; set; }
    public double Price { get; set; }
    public IList<Player> Players { get; set; } = [];
}