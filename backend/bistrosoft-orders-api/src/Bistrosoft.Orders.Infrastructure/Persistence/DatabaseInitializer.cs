using Bistrosoft.Orders.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bistrosoft.Orders.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();

        // Check if using InMemory database
        var isInMemory = context.Database.IsInMemory();

        if (isInMemory)
        {
            // For InMemory database, ensure schema is created
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }
        else
        {
            // For relational databases (SQL Server), apply pending migrations
            await context.Database.MigrateAsync(cancellationToken);
        }

        // Seed initial data
        await seeder.SeedAsync(cancellationToken);
    }
}
