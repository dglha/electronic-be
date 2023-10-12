using Electronic.Domain.Models.New;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.New;

public class NewItemConfiguration : IEntityTypeConfiguration<NewItem>
{
    public void Configure(EntityTypeBuilder<NewItem> builder)
    {
        builder.HasOne(ni => ni.ThumbnailImage)
            .WithOne()
            .HasForeignKey<NewItem>(ni => ni.ThumbnailImageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}