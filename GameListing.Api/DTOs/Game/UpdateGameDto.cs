using System.ComponentModel.DataAnnotations;

namespace GameListing.Api.DTOs.Game;

public class UpdateGameDto : CreateGameDto
{
    [Required]
    public int Id { get; set; }
}