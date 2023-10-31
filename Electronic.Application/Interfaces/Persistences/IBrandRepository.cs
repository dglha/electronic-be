using Electronic.Application.Contracts.Persistences;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Application.Interfaces.Persistences;

public interface IBrandRepository : IGenericRepository<Brand>
{
    string ConvertToSafeSlug(string slug);
    
}