using Electronic.Domain.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.TaxAmount)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.PaymentFeeAmount)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.ShippingFeeAmount)
            .HasPrecision(18, 2);
        
        builder.Property(o => o.OrderTotal)
            .HasPrecision(18, 2);
    }
}