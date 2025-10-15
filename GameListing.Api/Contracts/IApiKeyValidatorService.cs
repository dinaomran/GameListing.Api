namespace GameListing.Api.Contracts;

public interface IApiKeyValidatorService
{
    Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default);
}