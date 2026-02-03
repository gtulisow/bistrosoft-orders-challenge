using Bistrosoft.Orders.Domain.Entities;
using Bistrosoft.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bistrosoft.Orders.Tests.TestHelpers;

public static class InMemoryDbContextFactory
{
    public static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        
        // Seed OrderStatuses (required for tests)
        SeedOrderStatuses(context);
        
        return context;
    }

    private static void SeedOrderStatuses(AppDbContext context)
    {
        if (!context.OrderStatuses.Any())
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

            context.OrderStatuses.AddRange(orderStatuses);
            context.SaveChanges();
        }
    }

    public static AppDbContext CreateDbContextWithData(
        Action<AppDbContext> seedAction)
    {
        var context = CreateDbContext();
        seedAction(context);
        context.SaveChanges();
        return context;
    }
}
