using System.ComponentModel.DataAnnotations;

namespace GameListing.Api.DTOs.Game;

public class CreateGameDto
{
    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }
    [Required]
    [MaxLength(50)]
    public required string Category { get; set; }
    [Required]
    public DateOnly ReleaseDate { get; set; }
    [Required]
    [Range(0.99, 1000)]
    public double Price { get; set; }
    public List<int>? PlayerIds { get; set; }
}
