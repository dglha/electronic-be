using Electronic.Application.Interfaces.Persistences;
using Electronic.Domain.Models.Catalog;
using Electronic.Persistence.DatabaseContext;

namespace Electronic.Persistence.Interfaces.Repositories;

public class ProductOptionRepository : GenericRepository<ProductOption>, IProductOptionRepository
{
    private readonly ElectronicDatabaseContext _dbContext;

    public ProductOptionRepository(ElectronicDatabaseContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}