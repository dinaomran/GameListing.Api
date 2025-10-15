using GameListing.Api.Results;
using GameListing.Api.DTOs.Auth;

namespace GameListing.Api.Contracts
{
    public interface IUsersService
    {
        Task<Result<string>> LoginAsync(LoginDto loginDto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    }
}