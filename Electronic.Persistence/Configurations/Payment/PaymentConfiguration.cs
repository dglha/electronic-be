using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.Payment;

public class PaymentConfiguration : IEntityTypeConfiguration<Domain.Models.Payment.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Payment.Payment> builder)
    {
        builder.Property(p => p.PaymentFee).HasPrecision(18, 2);
        builder.Property(p => p.Amount).HasPrecision(18, 2);
    }
}