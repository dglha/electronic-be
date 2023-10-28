using Electronic.Application.Contracts.Logging;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Domain.Model.Catalog;
using Electronic.Persistence.DatabaseContext;

namespace Electronic.Persistence.Interfaces.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<Product> _logger;

    public ProductRepository(ElectronicDatabaseContext dbContext, IAppLogger<Product> logger) : base(dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
}