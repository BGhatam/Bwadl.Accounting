using Bwadl.Accounting.Domain.Entities;
using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Bwadl.Accounting.Infrastructure.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly ApiKeySettings _settings;
    private readonly IApiKeyRepository _apiKeyRepository;

    public ApiKeyService(IOptions<ApiKeySettings> settings, IApiKeyRepository apiKeyRepository)
    {
        _settings = settings.Value;
        _apiKeyRepository = apiKeyRepository;
    }

    public (string rawKey, string keyHash) GenerateApiKey()
    {
        // Generate random bytes for the key
        var keyBytes = new byte[_settings.KeyLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }

        // Create the raw key with prefix
        var rawKey = $"{_settings.Prefix}{Convert.ToBase64String(keyBytes).Replace("/", "_").Replace("+", "-").TrimEnd('=')}";
        
        // Hash the key for storage
        var keyHash = HashApiKey(rawKey);

        return (rawKey, keyHash);
    }

    public bool VerifyApiKey(string rawKey, string keyHash)
    {
        var computedHash = HashApiKey(rawKey);
        return computedHash == keyHash;
    }

    public async Task<bool> ValidateApiKeyAsync(string apiKey, string? ipAddress = null)
    {
        try
        {
            var keyHash = HashApiKey(apiKey);
            var apiKeyEntity = await _apiKeyRepository.GetByKeyHashAsync(keyHash);

            if (apiKeyEntity == null || !apiKeyEntity.IsValidKey)
            {
                return false;
            }

            // Check IP restrictions if enabled and configured
            if (_settings.EnableIpWhitelisting && !string.IsNullOrEmpty(apiKeyEntity.AllowedIpAddresses) && !string.IsNullOrEmpty(ipAddress))
            {
                var allowedIps = JsonSerializer.Deserialize<string[]>(apiKeyEntity.AllowedIpAddresses);
                if (allowedIps != null && allowedIps.Length > 0 && !allowedIps.Contains(ipAddress))
                {
                    return false;
                }
            }

            // Record usage
            await RecordUsageAsync(apiKeyEntity.Id);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ApiKey?> GetApiKeyAsync(string rawKey)
    {
        var keyHash = HashApiKey(rawKey);
        return await _apiKeyRepository.GetByKeyHashAsync(keyHash);
    }

    public async Task RecordUsageAsync(int apiKeyId)
    {
        var apiKey = await _apiKeyRepository.GetByIdAsync(apiKeyId);
        if (apiKey != null)
        {
            apiKey.RecordUsage();
            await _apiKeyRepository.UpdateAsync(apiKey);
        }
    }

    private string HashApiKey(string rawKey)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToBase64String(hashedBytes);
    }
}
