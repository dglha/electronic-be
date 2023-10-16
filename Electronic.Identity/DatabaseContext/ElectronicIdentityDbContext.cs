using Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity;

public class ElectronicIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public ElectronicIdentityDbContext(DbContextOptions<ElectronicIdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ElectronicIdentityDbContext).Assembly);
    }
}