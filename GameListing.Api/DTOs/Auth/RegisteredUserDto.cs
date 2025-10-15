namespace GameListing.Api.DTOs.Auth;

public class RegisteredUserDto // This DTO is used to send data so it has no password and any validations
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}