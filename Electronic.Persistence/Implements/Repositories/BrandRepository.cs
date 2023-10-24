using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Persistences;
using Electronic.Domain.Model.Catalog;
using Electronic.Persistence.DatabaseContext;

namespace Electronic.Persistence.Interfaces.Repositories;

public class BrandRepository : GenericRepository<Brand>,  IBrandRepository
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<Brand> _logger;

    public BrandRepository(ElectronicDatabaseContext dbContext, IAppLogger<Brand> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public string ConvertToSafeSlug(string slug)
    {
        var i = 2;
        while (true)
        {
            var isSlugExists = _dbContext.Set<Brand>().Any(b => b.Slug == slug);
            if (!isSlugExists) return slug;
            slug = $"{slug}-{i}";
            i++;
        }
    }
    
    
}