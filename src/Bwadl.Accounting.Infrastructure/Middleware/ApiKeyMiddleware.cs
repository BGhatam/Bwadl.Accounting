using Bwadl.Accounting.Domain.Interfaces;
using Bwadl.Accounting.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Bwadl.Accounting.Infrastructure.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeySettings _settings;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeySettings> settings)
    {
        _next = next;
        _settings = settings.Value;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        // Skip API key validation for certain paths
        if (ShouldSkipApiKeyValidation(context.Request.Path))
        {
            await _next(context);
            return;
        }

        // Check for API key in header
        if (!context.Request.Headers.TryGetValue(_settings.HeaderName, out var apiKeyHeaderValue))
        {
            // No API key provided, continue to JWT authentication
            await _next(context);
            return;
        }

        var apiKey = apiKeyHeaderValue.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            await WriteUnauthorizedResponse(context, "Invalid API key format");
            return;
        }

        // Validate API key
        var clientIpAddress = GetClientIpAddress(context);
        var isValid = await apiKeyService.ValidateApiKeyAsync(apiKey, clientIpAddress);

        if (!isValid)
        {
            await WriteUnauthorizedResponse(context, "Invalid or expired API key");
            return;
        }

        // Get API key details and set user context
        var apiKeyEntity = await apiKeyService.GetApiKeyAsync(apiKey);
        if (apiKeyEntity != null)
        {
            // Add API key information to HttpContext for later use
            context.Items["ApiKey"] = apiKeyEntity;
            context.Items["UserId"] = apiKeyEntity.UserId;
            
            // Set permissions from API key
            if (!string.IsNullOrEmpty(apiKeyEntity.Permissions))
            {
                try
                {
                    var permissions = JsonSerializer.Deserialize<string[]>(apiKeyEntity.Permissions);
                    context.Items["ApiKeyPermissions"] = permissions;
                }
                catch
                {
                    // Invalid permissions format, log and continue
                }
            }
        }

        await _next(context);
    }

    private static bool ShouldSkipApiKeyValidation(PathString path)
    {
        var pathsToSkip = new[]
        {
            "/health",
            "/swagger",
            "/api/v1/auth/login",
            "/api/v1/auth/register",
            "/api/v1/auth/refresh"
        };

        return pathsToSkip.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // Check for X-Forwarded-For header first (for load balancers/proxies)
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            // X-Forwarded-For can contain multiple IPs, take the first one
            return xForwardedFor.Split(',')[0].Trim();
        }

        // Check for X-Real-IP header
        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp))
        {
            return xRealIp;
        }

        // Fall back to connection remote IP address
        return context.Connection.RemoteIpAddress?.ToString();
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Unauthorized",
            message = message,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
