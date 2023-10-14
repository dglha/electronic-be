using Electronic.Domain.Model.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.ThumbnailImage)
            .WithOne()
            .HasForeignKey<Product>(p => p.ThumbnailImageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.OldPrice)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.SpecialPrice)
            .HasPrecision(18, 2);
    }
}