using Electronic.Application.Contracts.DTOs.Category;
using Electronic.Application.Contracts.DTOs.Category.Admin;
using Electronic.Application.Contracts.Response;
using Electronic.Domain.Models.Catalog;

namespace Electronic.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<CreateCategoryResultDto> CreateNewCategory(CreateCategoryDto request, Stream mediaBinaryStream, string fileName, string mimeType = null);
    Task<Pagination<CategoryDto>> GetAllCategories(int pageIndex, int itemPerPage);
    Task DeleteCategory(long categoryId);
    Task UpdateCategoryChildren(long parentCategoryId, List<long> childIds);

    Task<List<CategoryDto>> GetListAvailableCategory();
    Task<BaseResponse<CategoryDto>> UpdateCategory(long categoryId, UpdateCategoryRequestDto request);

    Task<CreateCategoryResultDto> GetCategoryDetailInfo(long categoryId);

    Task<BaseResponse<ICollection<CategoryListViewDto>>> GetCategoryTreeView();

    Task<BaseResponse<CategoryDto>> UpdateParentCategory(long categoryId, long parentCategoryId);
    
    Task<BaseResponse<CategoryDto>> RemoveParentCategory(long categoryId);
}