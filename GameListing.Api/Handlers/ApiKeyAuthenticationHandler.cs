using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using GameListing.Api.Application.Contracts;

namespace GameListing.Api.Handlers;

public class ApiKeyAuthenticationHandler    (
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyValidatorService apiKeyValidatorService
    ) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string apiKey = string.Empty;

        if (Request.Headers.TryGetValue("X-Api-Key", out var headerValues))
        {
            apiKey = headerValues.ToString();
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var valid = await apiKeyValidatorService.IsValidAsync(apiKey, Context.RequestAborted); // "Context.RequestAborted" means "if request is aborted == yes" then stop the IsValidAsync method

        if (!valid)
        {
            return AuthenticateResult.Fail("Invalid API Key.");
        }

        // Claims are bits of information about the user (e.g., username, email, roles, etc.)
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "apiKey"),
            new(ClaimTypes.Name, "ApiKeyClient")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}