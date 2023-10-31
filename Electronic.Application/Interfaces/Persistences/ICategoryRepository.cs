using Electronic.Application.Contracts.Persistences;
using Electronic.Domain.Models.Catalog;

namespace Electronic.Application.Interfaces.Persistences;

public interface ICategoryRepository : IGenericRepository<Category>
{
    public string ConvertToSafeSlug(string slug);
}