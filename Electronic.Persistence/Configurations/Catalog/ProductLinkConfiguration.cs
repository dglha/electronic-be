using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.Catalog;

public class ProductLinkConfiguration : IEntityTypeConfiguration<ProductLink>
{
    public void Configure(EntityTypeBuilder<ProductLink> builder)
    {
        builder.HasOne<Product>(pl => pl.Product)
            .WithMany(p => p.ProductLinks)
            .HasForeignKey(pl => pl.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<Product>(pl => pl.LinkedProduct)
            .WithMany()
            .HasForeignKey(pl => pl.LinkedProductId)
            .OnDelete(DeleteBehavior.Restrict);
            
    }
}