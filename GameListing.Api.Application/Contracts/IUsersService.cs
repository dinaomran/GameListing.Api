using GameListing.Api.Common.Results;
using GameListing.Api.Application.DTOs.Auth;

namespace GameListing.Api.Application.Contracts
{
    public interface IUsersService
    {
        Task<Result<string>> LoginAsync(LoginDto loginDto);
        Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    }
}