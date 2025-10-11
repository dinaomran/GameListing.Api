namespace GameListing.Api.Data;

public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public double Price { get; set; }
    public IList<Player> Players { get; set; } = [];
}