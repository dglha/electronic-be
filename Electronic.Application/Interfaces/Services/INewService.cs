using Electronic.Application.Contracts.DTOs.New;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Persistences;

public interface INewService
{
    public Task<NewCategoryDto> AddNewCategory(CreateNewCategoryDto request);
    public Task RemoveNewCategory(int newCategoryId);
    public Task AddNewItem(CreateNewItemDto request);
    public Task RemoveNewItem();
    public Task<Pagination<NewItemDto>> GetNewItems(int pageIndex, int itemPerPage);
    public Task<BaseResponse<ICollection<NewCategoryDto>>> GetNewCategories();
    public Task<BaseResponse<NewItemDetailDto>> GetNewItemDetails(int newItemId);
    
    public Task<Pagination<NewItemDto>> GetUserNewItems(int pageIndex, int itemPerPage, int categoryId);
    public Task<BaseResponse<ICollection<NewCategoryDto>>> GetUserNewCategories();
    public Task<BaseResponse<NewItemDetailDto>> GetNewItemDetailSlug(string slug);
}