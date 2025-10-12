namespace GameListing.Api.Data;

public class Player
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public IList<Game> Games { get; set; } = [];
}