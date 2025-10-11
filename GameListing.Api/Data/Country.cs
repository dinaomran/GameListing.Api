namespace GameListing.Api.Data;

public class Country
{
    public int CountryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<Player> Players { get; set; } = [];
}