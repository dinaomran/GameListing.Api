using System.ComponentModel.DataAnnotations;

namespace GameListing.Api.Application.DTOs.Game;

public class UpdateGameDto : CreateGameDto
{
    [Required]
    public int Id { get; set; }
}