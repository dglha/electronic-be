using Electronic.Application.Contracts.Persistences;
using Electronic.Domain.Models.Catalog;
using Electronic.Persistence.DatabaseContext;

namespace Electronic.Persistence.Interfaces.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly ElectronicDatabaseContext _dbContext;
    public CategoryRepository(ElectronicDatabaseContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    public string ConvertToSafeSlug(string slug)
    {
        var i = 2;
        while (true)
        {
            var isSlugExists = _dbContext.Set<Category>().Any(b => b.Slug == slug);
            if (!isSlugExists) return slug;
            slug = $"{slug}-{i}";
            i++;
        }
    }
}