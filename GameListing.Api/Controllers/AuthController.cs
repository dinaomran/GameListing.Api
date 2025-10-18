using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GameListing.Api.Application.DTOs.Auth;
using GameListing.Api.Application.Contracts;

namespace GameListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthController(IUsersService usersService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisteredUserDto>> Register(RegisterUserDto registerUserDto)
    {
        var result = await usersService.RegisterAsync(registerUserDto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
    {
        var result = await usersService.LoginAsync(loginDto);
        return ToActionResult(result);
    }
}