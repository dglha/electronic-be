using Electronic.Application.Contracts.DTOs.Category;
using Electronic.Application.Contracts.Response;
using Electronic.Domain.Models.Catalog;

namespace Electronic.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<CreateCategoryResultDto> CreateNewCategory(CreateCategoryDto request);
    Task<Pagination<CategoryDto>> GetAllCategories(int pageIndex, int itemPerPage);
    Task DeleteCategory(long categoryId);
}