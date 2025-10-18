using GameListing.Api.Application.Contracts;
using GameListing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace GameListing.Api.Application.Services;

// Validate Api Keys from database
public class ApiKeyValidatorService(GameListingDbContext db) : IApiKeyValidatorService
{
    public async Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return false;

        var apiKeyEntity = await db.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Key == apiKey, ct);

        if (apiKeyEntity is null)
            return false;

        // If there is no expiration date or the expiration date doesn't exceed today's date
        return apiKeyEntity.IsActive;
    }
}

//// Validate Api Keys from configuration file
//public class ApiKeyValidatorService(IConfiguration configuration) : IApiKeyValidatorService
//{
//    public Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
//    {
//        return Task.FromResult(apiKey.Equals(configuration["ApiKey"]));
//    }
//}