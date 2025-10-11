namespace GameListing.Api.Data;

public class Player
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public Country? Country { get; set; }
    public IList<Game> Games { get; set; } = [];
}