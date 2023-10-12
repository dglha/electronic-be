using Electronic.Domain.Models.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.CountryId);

        builder.Property(c => c.Name)
            .HasMaxLength(450);
        
        builder.Property(c => c.Code3)
            .HasMaxLength(450);

        builder.HasMany(c => c.StateOrProvinces)
            .WithOne(s => s.Country)
            .OnDelete(DeleteBehavior.Restrict);
    }
}