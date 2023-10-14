using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.Catalog;

public class ProductMediaConfiguration : IEntityTypeConfiguration<ProductMedia>
{
    public void Configure(EntityTypeBuilder<ProductMedia> builder)
    {
        builder.HasKey(pm => new { pm.ProductId, pm.MediaId });
        
        // builder.HasOne(pm => pm.Media)
        //     .WithOne()
        //     .HasForeignKey<ProductMedia>(pm => pm.MediaId)
        //     .OnDelete(DeleteBehavior.Restrict);
        //
        // builder.HasOne(pm => pm.Product)
        //     .WithOne()
        //     .HasForeignKey<ProductMedia>(pm => pm.ProductId)
        //     .OnDelete(DeleteBehavior.Restrict);
    }
}