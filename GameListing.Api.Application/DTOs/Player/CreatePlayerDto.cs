using System.ComponentModel.DataAnnotations;

namespace GameListing.Api.Application.DTOs.Player;

public class CreatePlayerDto
{
    [Required]
    public required string Username { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    public int CountryId { get; set; }
    public List<int>? GameIds { get; set; }
}
