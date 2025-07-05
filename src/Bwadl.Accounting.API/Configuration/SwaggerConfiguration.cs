using Microsoft.OpenApi.Models;

namespace Bwadl.Accounting.API.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Bwadl API V1",
                Version = "v1",
                Description = "Bwadl Accounting API",
                Contact = new OpenApiContact
                {
                    Name = "Development Team",
                    Email = "balghatam@bwadl.sa"
                }
            });



            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // Remove global security requirement and let individual endpoints define their own
            // This allows Swagger to properly show locked/unlocked icons based on [Authorize] attributes
            
            // Add operation filter to handle authorization requirements per endpoint
            c.OperationFilter<AuthorizeOperationFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bwadl API V1");
            c.RoutePrefix = "swagger"; // Change from empty to swagger
            c.DocumentTitle = "Bwadl API Documentation";
        });

        return app;
    }
}
