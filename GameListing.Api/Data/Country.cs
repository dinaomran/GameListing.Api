namespace GameListing.Api.Data;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public IList<Player> Players { get; set; } = [];
}