using Electronic.Domain.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(o => o.TaxAmount)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(o => o.ProductPrice)
            .HasPrecision(18, 2);

        builder.Property(o => o.TaxPercent)
            .HasPrecision(18, 2);
    }
}