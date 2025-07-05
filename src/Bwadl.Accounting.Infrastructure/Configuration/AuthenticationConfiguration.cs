using Bwadl.Accounting.Domain.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Bwadl.Accounting.Infrastructure.Configuration;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT settings
        var jwtSettings = new JwtSettings();
        configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Configure API key settings
        var apiKeySettings = new ApiKeySettings();
        configuration.GetSection(ApiKeySettings.SectionName).Bind(apiKeySettings);
        services.Configure<ApiKeySettings>(configuration.GetSection(ApiKeySettings.SectionName));

        // Add JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = jwtSettings.Issuer.StartsWith("https://");
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = jwtSettings.ValidateIssuer,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = jwtSettings.ValidateAudience,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = jwtSettings.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes)
            };

            // Add custom event handlers
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = System.Text.Json.JsonSerializer.Serialize(new 
                        { 
                            error = "Unauthorized", 
                            message = "Invalid or missing JWT token",
                            timestamp = DateTime.UtcNow
                        });
                        return context.Response.WriteAsync(result, cancellationToken: default);
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Role-based policies
            options.AddPolicy("RequireSuperAdminRole", policy => 
                policy.RequireRole("SuperAdmin"));
            
            options.AddPolicy("RequireAdminRole", policy => 
                policy.RequireRole("SuperAdmin", "Admin"));
            
            options.AddPolicy("RequireUserRole", policy => 
                policy.RequireRole("SuperAdmin", "Admin", "User"));

            // Permission-based policies
            options.AddPolicy("Users.Read", policy => 
                policy.RequireClaim("permission", "Users.Read"));
            
            options.AddPolicy("Users.Write", policy => 
                policy.RequireClaim("permission", "Users.Write"));
            
            options.AddPolicy("Users.Delete", policy => 
                policy.RequireClaim("permission", "Users.Delete"));

            options.AddPolicy("Admin.Read", policy => 
                policy.RequireClaim("permission", "Admin.Read"));
            
            options.AddPolicy("Admin.Write", policy => 
                policy.RequireClaim("permission", "Admin.Write"));
            
            options.AddPolicy("Admin.Delete", policy => 
                policy.RequireClaim("permission", "Admin.Delete"));

            options.AddPolicy("System.Read", policy => 
                policy.RequireClaim("permission", "System.Read"));
            
            options.AddPolicy("System.Write", policy => 
                policy.RequireClaim("permission", "System.Write"));
            
            options.AddPolicy("System.Delete", policy => 
                policy.RequireClaim("permission", "System.Delete"));

            options.AddPolicy("ApiKeys.Read", policy => 
                policy.RequireClaim("permission", "ApiKeys.Read"));
            
            options.AddPolicy("ApiKeys.Write", policy => 
                policy.RequireClaim("permission", "ApiKeys.Write"));
            
            options.AddPolicy("ApiKeys.Delete", policy => 
                policy.RequireClaim("permission", "ApiKeys.Delete"));

            // Combined policies
            options.AddPolicy("ManageUsers", policy => 
                policy.RequireAssertion(context =>
                    context.User.IsInRole("SuperAdmin") ||
                    context.User.IsInRole("Admin") ||
                    context.User.HasClaim("permission", "Users.Write")));

            options.AddPolicy("ViewUsers", policy => 
                policy.RequireAssertion(context =>
                    context.User.IsInRole("SuperAdmin") ||
                    context.User.IsInRole("Admin") ||
                    context.User.IsInRole("User") ||
                    context.User.HasClaim("permission", "Users.Read")));
        });

        return services;
    }
}
