using Bistrosoft.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bistrosoft.Orders.Infrastructure.Configurations;

public class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("OrderStatuses");

        builder.HasKey(os => os.Id);

        builder.Property(os => os.Id)
            .ValueGeneratedNever(); // IDs are predefined

        builder.Property(os => os.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(os => os.Name)
            .IsUnique();

        builder.Property(os => os.Description)
            .HasMaxLength(200);

        // Relationship with Orders
        builder.HasMany(os => os.Orders)
            .WithOne(o => o.Status)
            .HasForeignKey(o => o.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
