using System.Text;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using GameListing.Api.Application.Contracts;
using GameListing.Api.Application.DTOs.Auth;

namespace GameListing.Api.Handlers;

public class BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUsersService usersService
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        // Basic Base64({username:password}) -> This is the format of the auth that is in the header
        var authHeader = authHeaderValues.ToString();
        if(string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        // Get Token

        var token = authHeader["Basic ".Length..].Trim(); // Get everything that comes after whatever the length of "Basic " and get rid of any whitespace
        string decoded;

        try
        {
            var credentialBytes = Convert.FromBase64String(token);
            // {username:password} -> This is the format of the decoded token
            decoded = Encoding.UTF8.GetString(credentialBytes);
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Basic Authentication Token.");
        }

        var separatorIndex = decoded.IndexOf(':');
        if (separatorIndex <= 0 )
        {
            return AuthenticateResult.Fail("Invalid Basic Authentication Credentials Format.");
        }

        var usernameOrEmail = decoded[..separatorIndex];
        var password = decoded[(separatorIndex + 1)..];

        var loginDto = new LoginDto
        {
            Email = usernameOrEmail,
            Password = password
        };

        var result = await usersService.LoginAsync(loginDto);

        if (!result.IsSuccess)
        {
            return AuthenticateResult.Fail("Invalid Username or Password.");
        }

        // Claims are bits of information about the user (e.g., username, email, roles, etc.)
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usernameOrEmail)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}