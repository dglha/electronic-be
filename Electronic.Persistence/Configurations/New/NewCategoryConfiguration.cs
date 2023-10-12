using Electronic.Domain.Models.New;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Electronic.Persistence.Configurations.New;

public class NewCategoryConfiguration : IEntityTypeConfiguration<NewCategory>
{
    public void Configure(EntityTypeBuilder<NewCategory> builder)
    {
       
    }
}