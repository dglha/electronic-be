using Electronic.Domain.Model.Catalog;

namespace Electronic.Application.Contracts.Persistences;

public interface IBrandRepository : IGenericRepository<Brand>
{
    string ConvertToSafeSlug(string slug);
    
}