using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        builder.HasData(
            new ApplicationUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                Email = "dlha.19it1@vku.udn.vn",
                NormalizedEmail = "DLHA.19IT1@VKU.UDN.VN",
                UserName = "duongleha",
                NormalizedUserName = "DUONGLEHA",
                PasswordHash = hasher.HashPassword(null, "Ha02012001"),
                EmailConfirmed = true
            }
        );
    }
}