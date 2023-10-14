using Electronic.Domain.Models.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.Catalog;

public class ProductPriceHistoryConfiguration : IEntityTypeConfiguration<ProductPriceHistory>
{
    public void Configure(EntityTypeBuilder<ProductPriceHistory> builder)
    {
        builder.HasOne(ppr => ppr.Product)
            .WithMany(p => p.PriceHistories)
            .HasForeignKey(ppr => ppr.ProductId);
        
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.OldPrice)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.SpecialPrice)
            .HasPrecision(18, 2);
    }
}