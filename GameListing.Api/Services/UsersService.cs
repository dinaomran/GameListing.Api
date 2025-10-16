using System.Text;
using GameListing.Api.Data;
using System.Security.Claims;
using GameListing.Api.Results;
using GameListing.Api.Constants;
using GameListing.Api.Contracts;
using GameListing.Api.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace GameListing.Api.Services;

public class UsersService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            UserName = registerUserDto.Email
        };

        var result = await userManager.CreateAsync(user, registerUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();

            return Result<RegisteredUserDto>.BadRequest(errors);
        }

        // Assign Role to User
        await userManager.AddToRoleAsync(user, registerUserDto.Role);

        var registeredUser = new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = registerUserDto.Role
        };

        return Result<RegisteredUserDto>.Success(registeredUser);
    }

    public async Task<Result<string>> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return Result<string>.BadRequest(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordValid)
        {
            return Result<string>.BadRequest(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));
        }

        // Issue JWT Token
        var token = await GenerateToken(user);

        //return Result<string>.Success("Logged In Successfully");
        return Result<string>.Success(token);
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        // Set basic user claims
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id), // subject
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Represents random value every time token is generated
            new (JwtRegisteredClaimNames.Name, user.FullName)
        };

        // Set user role claims (Example: i want specific user role to access get countries api)
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // Set JWT key credentials
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create an encoded token
        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])), // Don't use "DateTime.Now" it can cause issues when server is in different timezone
            signingCredentials: credentials
        );

        // Return token value
        return new JwtSecurityTokenHandler().WriteToken(token); // returns string value of token
    }
}