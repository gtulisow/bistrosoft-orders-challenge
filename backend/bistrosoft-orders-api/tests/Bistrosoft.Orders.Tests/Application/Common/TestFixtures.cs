namespace Bistrosoft.Orders.Tests.Application.Common;

public static class TestFixtures
{
    public static class Customers
    {
        public static readonly Guid ValidCustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public const string ValidName = "John Doe";
        public const string ValidEmail = "john.doe@example.com";
        public const string ValidPhone = "+1234567890";
    }

    public static class Products
    {
        public static readonly Guid ValidProductId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public const string ValidName = "Sample Product";
        public const decimal ValidPrice = 100m;
        public const int ValidStock = 50;
    }

    public static class Orders
    {
        public static readonly Guid ValidOrderId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public const decimal ValidTotalAmount = 250m;
        public const int ValidQuantity = 2;
    }
}
