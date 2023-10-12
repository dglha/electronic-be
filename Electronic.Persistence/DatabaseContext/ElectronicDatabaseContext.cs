using Electronic.Domain.Models.Core;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.DatabaseContext;

public class ElectronicDatabaseContext : DbContext
{
    public ElectronicDatabaseContext(DbContextOptions<ElectronicDatabaseContext> options) : base (options)
    {
        
    }

    #region Core

    public DbSet<District> Districts { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<StateOrProvince> StateOrProvinces { get; set; }
    public DbSet<Media> Media { get; set; }
    public DbSet<Address> Addresses { get; set; }

    #endregion
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElectronicDatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}