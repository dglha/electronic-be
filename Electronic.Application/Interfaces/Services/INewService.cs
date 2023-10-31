using Electronic.Application.Contracts.DTOs.New;

namespace Electronic.Application.Interfaces.Persistences;

public interface INewService
{
    public Task<NewCategoryDto> AddNewCategory(CreateNewCategoryDto request);
    public Task RemoveNewCategory(int newCategoryId);

    public Task AddNewItem();
    public Task RemoveNewItem();
}