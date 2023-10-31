using Electronic.Domain.Models.New;

namespace Electronic.Application.Interfaces.Persistences;

public interface INewCategoryRepository : IGenericRepository<NewCategory>
{
    string ConvertToSafeSlug(string slug);
}