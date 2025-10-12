using System.ComponentModel.DataAnnotations;

namespace GameListing.Api.DTOs.Country;

public class CreateCountryDto
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }
    public List<int>? PlayerIds { get; set; }
}
