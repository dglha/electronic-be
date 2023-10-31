using Electronic.Application.Contracts.DTOs.New;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Domain.Models.New;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Helpers;

namespace Electronic.Persistence.Implements.Services;

public class NewService : INewService
{
    private readonly INewCategoryRepository _newCategoryRepository;
    private readonly ElectronicDatabaseContext _dbContext;

    public NewService(INewCategoryRepository newCategoryRepository, ElectronicDatabaseContext dbContext)
    {
        _newCategoryRepository = newCategoryRepository;
        _dbContext = dbContext;
    }

    public async Task<NewCategoryDto> AddNewCategory(CreateNewCategoryDto request)
    {
        var slug = SlugGenerator.Generate(request.Name);
        var safeSlug = _newCategoryRepository.ConvertToSafeSlug(slug);
        var newCategory = new NewCategory
        {
            Name = request.Name, 
            Description = request.Description, 
            Slug = safeSlug,
            IsPublished = (bool)request.IsPublished!,
            IsDeleted = false,
            DisplayOrder = (int)request.DisplayOrder!
        };

        await _newCategoryRepository.CreateAsync(newCategory);
        
        return new NewCategoryDto
        {
            Name = newCategory.Name,
            Slug = newCategory.Slug,
            Description = newCategory.Description,
            IsPublished = newCategory.IsPublished,
            DisplayOrder = newCategory.DisplayOrder,
            NewCategoryId = newCategory.NewCategoryId
        };
    }

    public Task RemoveNewCategory(int newCategoryId)
    {
        throw new NotImplementedException();
    }

    public Task AddNewItem()
    {
        throw new NotImplementedException();
    }

    public Task RemoveNewItem()
    {
        throw new NotImplementedException();
    }
}