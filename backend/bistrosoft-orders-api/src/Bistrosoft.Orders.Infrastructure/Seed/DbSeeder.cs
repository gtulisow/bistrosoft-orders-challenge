using Bistrosoft.Orders.Application.Interfaces;
using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Domain.ValueObjects;
using Bistrosoft.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bistrosoft.Orders.Infrastructure.Seed;

public class DbSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(
        AppDbContext context,
        IPasswordService passwordService,
        ILogger<DbSeeder> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Seed OrderStatuses if none exist
        if (!await _context.OrderStatuses.AnyAsync(cancellationToken))
        {
            var orderStatuses = new List<OrderStatus>
            {
                new OrderStatus(
                    OrderStatus.WellKnownStatuses.Pending,
                    "Pending",
                    "Order has been created but not yet paid"),
                new OrderStatus(
                    OrderStatus.WellKnownStatuses.Paid,
                    "Paid",
                    "Payment has been received and confirmed"),
                new OrderStatus(
                    OrderStatus.WellKnownStatuses.Shipped,
                    "Shipped",
                    "Order has been shipped and is on the way"),
                new OrderStatus(
                    OrderStatus.WellKnownStatuses.Delivered,
                    "Delivered",
                    "Order has been successfully delivered to the customer"),
                new OrderStatus(
                    OrderStatus.WellKnownStatuses.Cancelled,
                    "Cancelled",
                    "Order has been cancelled by the customer or system")
            };

            await _context.OrderStatuses.AddRangeAsync(orderStatuses, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Seed Products if none exist
        if (!await _context.Products.AnyAsync(cancellationToken))
        {
            var products = new List<Product>
            {
                // PIZZAS (12 productos)
                new Product("Pizza Margarita", 12.50m, 80),
                new Product("Pizza Pepperoni", 13.90m, 75),
                new Product("Pizza Cuatro Quesos", 14.50m, 60),
                new Product("Pizza Hawaiana", 13.50m, 70),
                new Product("Pizza Vegetariana", 13.00m, 65),
                new Product("Pizza Barbacoa", 15.50m, 55),
                new Product("Pizza Napolitana", 14.00m, 60),
                new Product("Pizza Diavola", 14.90m, 50),
                new Product("Pizza Prosciutto", 15.90m, 45),
                new Product("Pizza Romana", 13.90m, 60),
                new Product("Pizza Funghi", 13.50m, 55),
                new Product("Pizza Capricciosa", 15.50m, 50),

                // PASTAS (10 productos)
                new Product("Pasta Carbonara", 11.50m, 90),
                new Product("Pasta Bolognesa", 11.00m, 95),
                new Product("Pasta Alfredo", 12.50m, 70),
                new Product("Pasta Arrabiata", 10.50m, 80),
                new Product("Pasta Pesto", 11.90m, 75),
                new Product("Lasagna de Carne", 13.50m, 60),
                new Product("Ravioles de Ricota", 12.90m, 55),
                new Product("Pasta Frutti di Mare", 16.50m, 40),
                new Product("Pasta Puttanesca", 11.50m, 65),
                new Product("Pasta Primavera", 10.90m, 70),

                // HAMBURGUESAS (10 productos)
                new Product("Hamburguesa Clásica", 9.50m, 100),
                new Product("Hamburguesa Doble Queso", 11.90m, 90),
                new Product("Hamburguesa BBQ", 12.50m, 75),
                new Product("Hamburguesa Bacon", 12.90m, 80),
                new Product("Hamburguesa Vegetariana", 10.50m, 70),
                new Product("Hamburguesa Blue Cheese", 13.50m, 60),
                new Product("Hamburguesa Mexicana", 12.90m, 65),
                new Product("Hamburguesa Pollo Crispy", 11.50m, 85),
                new Product("Hamburguesa Portobello", 11.90m, 55),
                new Product("Hamburguesa Premium Angus", 15.90m, 45),

                // TACOS (8 productos)
                new Product("Tacos al Pastor", 9.90m, 120),
                new Product("Tacos de Carne Asada", 10.50m, 110),
                new Product("Tacos de Pescado", 11.50m, 80),
                new Product("Tacos de Pollo", 9.50m, 100),
                new Product("Tacos Vegetarianos", 8.90m, 90),
                new Product("Tacos de Carnitas", 10.90m, 95),
                new Product("Tacos de Barbacoa", 11.90m, 70),
                new Product("Tacos de Camarón", 12.50m, 60),

                // ENSALADAS (8 productos)
                new Product("Ensalada César", 7.90m, 85),
                new Product("Ensalada Griega", 8.50m, 75),
                new Product("Ensalada Caprese", 9.50m, 60),
                new Product("Ensalada de Pollo", 10.50m, 80),
                new Product("Ensalada Mixta", 6.90m, 90),
                new Product("Ensalada de Atún", 9.90m, 70),
                new Product("Ensalada Tropical", 8.90m, 65),
                new Product("Ensalada Quinoa", 10.90m, 55),

                // SÁNDWICHES (8 productos)
                new Product("Sándwich Club", 8.50m, 95),
                new Product("Sándwich de Pollo", 7.90m, 100),
                new Product("Sándwich Vegetariano", 7.50m, 80),
                new Product("Sándwich de Jamón y Queso", 6.90m, 110),
                new Product("Sándwich BLT", 8.90m, 85),
                new Product("Sándwich de Atún", 8.50m, 75),
                new Product("Sándwich Pastrami", 9.90m, 60),
                new Product("Sándwich Cubano", 10.50m, 55),

                // SUSHI (10 productos)
                new Product("Sushi Roll California", 14.50m, 70),
                new Product("Sushi Roll Spicy Tuna", 15.90m, 60),
                new Product("Sushi Roll Philadelphia", 14.90m, 65),
                new Product("Sushi Roll Tempura", 15.50m, 55),
                new Product("Sashimi Mixto", 18.50m, 40),
                new Product("Nigiri Salmón (6 pzas)", 12.90m, 75),
                new Product("Nigiri Atún (6 pzas)", 13.50m, 70),
                new Product("Roll Dragón", 16.90m, 50),
                new Product("Roll Rainbow", 17.50m, 45),
                new Product("Gyoza (6 pzas)", 8.50m, 90),

                // PARRILLA / BBQ (10 productos)
                new Product("Costillas BBQ", 18.50m, 55),
                new Product("Brisket Ahumado", 19.90m, 45),
                new Product("Pollo a la Parrilla", 14.50m, 80),
                new Product("Churrasco", 21.50m, 40),
                new Product("Asado de Tira", 22.90m, 35),
                new Product("Picaña", 23.50m, 30),
                new Product("Brochetas Mixtas", 16.50m, 60),
                new Product("Alitas BBQ (12 pzas)", 11.90m, 100),
                new Product("Chorizo Argentino", 9.90m, 85),
                new Product("Parrillada Mixta (2 personas)", 45.00m, 25),

                // POSTRES (10 productos)
                new Product("Tiramisú", 6.50m, 70),
                new Product("Cheesecake", 6.90m, 75),
                new Product("Brownie con Helado", 7.50m, 80),
                new Product("Flan Casero", 5.50m, 90),
                new Product("Helado (3 bolas)", 5.90m, 100),
                new Product("Tarta de Manzana", 6.50m, 65),
                new Product("Panna Cotta", 6.90m, 60),
                new Product("Profiteroles", 7.90m, 55),
                new Product("Mousse de Chocolate", 6.50m, 70),
                new Product("Crema Catalana", 5.90m, 75),

                // BEBIDAS (20 productos)
                new Product("Agua Mineral", 2.00m, 150),
                new Product("Refresco", 2.50m, 140),
                new Product("Jugo Natural Naranja", 3.50m, 100),
                new Product("Jugo Natural Fresa", 3.50m, 95),
                new Product("Limonada Natural", 3.00m, 110),
                new Product("Té Helado", 2.90m, 120),
                new Product("Café Espresso", 2.50m, 130),
                new Product("Café Americano", 2.90m, 125),
                new Product("Café Capuccino", 3.50m, 115),
                new Product("Café Latte", 3.90m, 110),
                new Product("Cerveza Nacional", 3.50m, 140),
                new Product("Cerveza Importada", 4.50m, 100),
                new Product("Cerveza Artesanal IPA", 5.50m, 80),
                new Product("Cerveza Artesanal Stout", 5.90m, 70),
                new Product("Copa de Vino Tinto", 6.50m, 90),
                new Product("Copa de Vino Blanco", 6.50m, 85),
                new Product("Margarita", 7.50m, 75),
                new Product("Mojito", 7.90m, 80),
                new Product("Piña Colada", 8.50m, 70),
                new Product("Smoothie Tropical", 5.50m, 95)
            };

            await _context.Products.AddRangeAsync(products, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Seed Admin User if none exist (only if SEED_ADMIN_PASSWORD is set)
        if (!await _context.Users.AnyAsync(cancellationToken))
        {
            var seedPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(seedPassword))
            {
                _logger.LogWarning(
                    "SEED_ADMIN_PASSWORD environment variable not set. " +
                    "Skipping admin user seed. Set this variable to create a default admin user.");
            }
            else
            {
                var adminEmail = new Email("admin@bistrosoft.local");
                var passwordHash = _passwordService.HashPassword(seedPassword);
                var adminUser = new User(adminEmail, passwordHash);

                await _context.Users.AddAsync(adminUser, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Admin user created: {Email}", adminEmail.Value);
            }
        }
    }
}
