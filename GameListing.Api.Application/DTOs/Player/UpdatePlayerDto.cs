using System.ComponentModel.DataAnnotations;

namespace GameListing.Api.Application.DTOs.Player;

public class UpdatePlayerDto : CreatePlayerDto
{
    [Required]
    public int Id { get; set; }
}
