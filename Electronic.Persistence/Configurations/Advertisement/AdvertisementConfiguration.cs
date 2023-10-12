using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.Advertisement;

public class AdvertisementConfiguration : IEntityTypeConfiguration<Domain.Models.Advertisement>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Advertisement> builder)
    {
        builder.HasOne(a => a.Media)
            .WithOne()
            .HasForeignKey<Domain.Models.Advertisement>(a => a.MediaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}