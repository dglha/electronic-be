using Electronic.Domain.Models.Catalog;

namespace Electronic.Application.Contracts.Persistences;

public interface ICategoryRepository : IGenericRepository<Category>
{
    public string ConvertToSafeSlug(string slug);
}