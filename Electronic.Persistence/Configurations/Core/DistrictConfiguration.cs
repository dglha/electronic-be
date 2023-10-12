using Electronic.Domain.Models.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations;

public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.HasOne(d => d.StateOrProvince)
            .WithMany(s => s.Districts)
            .OnDelete(DeleteBehavior.Restrict);
    }
}