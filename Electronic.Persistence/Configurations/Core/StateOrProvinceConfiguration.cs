using Electronic.Domain.Models.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations;

public class StateOrProvinceConfiguration : IEntityTypeConfiguration<StateOrProvince>
{
    public void Configure(EntityTypeBuilder<StateOrProvince> builder)
    {
        builder.HasMany(s => s.Districts)
            .WithOne(d => d.StateOrProvince)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Country)
            .WithMany(c => c.StateOrProvinces)
            .OnDelete(DeleteBehavior.Restrict);
    }
}