namespace Bwadl.Accounting.Domain.Interfaces;

public interface IApiKeyService
{
    /// <summary>
    /// Generates a new API key
    /// </summary>
    /// <returns>A tuple containing the raw key and its hash</returns>
    (string rawKey, string keyHash) GenerateApiKey();

    /// <summary>
    /// Verifies an API key against its hash
    /// </summary>
    /// <param name="rawKey">The raw API key</param>
    /// <param name="keyHash">The stored hash</param>
    /// <returns>True if the key is valid</returns>
    bool VerifyApiKey(string rawKey, string keyHash);

    /// <summary>
    /// Validates an API key and returns whether it's valid
    /// </summary>
    /// <param name="apiKey">The API key to validate</param>
    /// <param name="ipAddress">The IP address making the request</param>
    /// <returns>True if the API key is valid and allowed</returns>
    Task<bool> ValidateApiKeyAsync(string apiKey, string? ipAddress = null);

    /// <summary>
    /// Gets the API key entity by raw key
    /// </summary>
    /// <param name="rawKey">The raw API key</param>
    /// <returns>The API key entity if found</returns>
    Task<Domain.Entities.ApiKey?> GetApiKeyAsync(string rawKey);

    /// <summary>
    /// Records API key usage
    /// </summary>
    /// <param name="apiKeyId">The API key ID</param>
    Task RecordUsageAsync(int apiKeyId);
}
