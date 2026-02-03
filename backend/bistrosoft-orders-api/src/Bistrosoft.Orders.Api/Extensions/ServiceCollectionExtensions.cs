using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Bistrosoft.Orders.Api.Extensions;

/// <summary>
/// Extension methods for IServiceCollection to configure API-specific services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds CORS policy to the service collection with configuration from appsettings
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var allowCredentials = configuration.GetValue<bool>("Cors:AllowCredentials");

        services.AddCors(options =>
        {
            options.AddPolicy("ApiCorsPolicy", policy =>
            {
                // Configure allowed origins
                if (allowedOrigins.Length > 0 && allowedOrigins[0] == "*")
                {
                    // Allow all origins (Development only)
                    policy.AllowAnyOrigin();
                }
                else if (allowedOrigins.Length > 0)
                {
                    // Allow specific origins
                    policy.WithOrigins(allowedOrigins);
                }
                else
                {
                    // If no origins configured, don't allow any (safe default for production)
                    policy.WithOrigins();
                }

                // Configure credentials support
                // Note: AllowCredentials cannot be used with AllowAnyOrigin
                if (allowCredentials && !(allowedOrigins.Length > 0 && allowedOrigins[0] == "*"))
                {
                    policy.AllowCredentials();
                }

                // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                policy.AllowAnyMethod();

                // Allow any headers from the client
                policy.AllowAnyHeader();

                // Expose common headers to the client if needed
                policy.WithExposedHeaders("Content-Disposition", "X-Total-Count", "X-Pagination");

                // Cache preflight requests for 1 hour to reduce OPTIONS traffic
                policy.SetPreflightMaxAge(TimeSpan.FromHours(1));
            });
        });

        return services;
    }

    /// <summary>
    /// Adds JWT Bearer authentication to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured");
        var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured");
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");

        if (key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be at least 32 characters long for security.");
        }

        var keyBytes = Encoding.UTF8.GetBytes(key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false; // Set to true in production
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero // Remove default 5-minute tolerance
            };
        });

        // Add Authorization
        // Note: Instead of FallbackPolicy, we use [Authorize] attribute on controllers
        // to avoid blocking Swagger UI and other static content
        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Configures Swagger to support JWT Bearer authentication
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token in the format: Bearer {token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
