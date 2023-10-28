using Electronic.Application.Interfaces.Persistences;
using Electronic.Domain.Models.New;
using Electronic.Persistence.DatabaseContext;

namespace Electronic.Persistence.Interfaces.Repositories;

public class NewCategoryRepository : GenericRepository<NewCategory>, INewCategoryRepository
{
    private readonly ElectronicDatabaseContext _dbContext;
    public NewCategoryRepository(ElectronicDatabaseContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public string ConvertToSafeSlug(string slug)
    {
        var i = 2;
        while (true)
        {
            var isSlugExists = _dbContext.Set<NewCategory>().Any(nc => nc.Slug == slug);
            if (!isSlugExists) return slug;
            slug = $"{slug}-{i}";
            i++;
        }
    }
}