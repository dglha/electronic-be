using Electronic.Domain.Models.New;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.New;

public class NewItemNewCategoryConfiguration : IEntityTypeConfiguration<NewItemNewCategory>
{
    public void Configure(EntityTypeBuilder<NewItemNewCategory> builder)
    {
        builder.HasKey(nn => new { nn.NewItemId, nn.NewCategoryId });
        
        builder.HasOne<NewItem>(nn => nn.NewItem)
            .WithMany(n => n.NewItemNewCategories)
            .HasForeignKey(nn => nn.NewItemId);
        
        builder.HasOne<NewCategory>(nn => nn.NewCategory)
            .WithMany(n => n.NewItemNewCategories)
            .HasForeignKey(nn => nn.NewCategoryId);
    }
}