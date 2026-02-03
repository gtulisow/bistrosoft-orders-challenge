using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Infrastructure.Repositories;
using Bistrosoft.Orders.Infrastructure.Seed;
using Bistrosoft.Orders.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bistrosoft.Orders.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Check if InMemory database should be used
        var useInMemoryStr = configuration["Database:UseInMemory"];
        var useInMemory = bool.TryParse(useInMemoryStr, out var result) && result;

        if (useInMemory)
        {
            // Register DbContext with InMemory provider
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("BistrosoftOrdersDb"));
        }
        else
        {
            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Register DbContext with SQL Server
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Register Repositories (Adapters)
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Security Services
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Register Seeder
        services.AddScoped<DbSeeder>();

        return services;
    }
}
