using System.Net;
using Electronic.Application.Contracts.DTOs.Category;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Persistences;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Catalog;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class CategoryService : ICategoryService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<CategoryService> _logger;
    private readonly ICategoryRepository _repository;

    public CategoryService(ElectronicDatabaseContext dbContext, IAppLogger<CategoryService> logger,
        ICategoryRepository repository)
    {
        _dbContext = dbContext;
        _logger = logger;
        _repository = repository;
    }

    public async Task<CreateCategoryResultDto> CreateNewCategory(CreateCategoryDto request)
    {
        var cateSlug = _repository.ConvertToSafeSlug(SlugGenerator.Generate(request.Name));
        var category = new Category
        {
            Name = request.Name,
            Slug = cateSlug,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            IsPublished = request.IsPublished,
            DisplayOrder = (int)request.DisplayOrder!,
            IncludeInMenu = (bool)request.IncludeInMenu!,
            IsDeleted = false
        };

        await _repository.CreateAsync(category);
        _logger.LogWarning("Need to implement media image feature!!!");
        return new CreateCategoryResultDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            Slug = category.Slug,
            IsPublished = category.IsPublished,
            DisplayOrder = category.DisplayOrder,
            IsDeleted = category.IsDeleted,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            IncludeInMenu = category.IncludeInMenu
        };
    }

    public async Task<Pagination<CategoryDto>> GetAllCategories(int pageNumber, int itemPerPage)
    {
        var query = GetListCategoryDtosQuery();
        var totalCount = await query.CountAsync();
        var data = await query.Skip((pageNumber - 1) * itemPerPage).Take(itemPerPage).ToListAsync();
        return Pagination<CategoryDto>.ToPagination(data, pageNumber, itemPerPage, totalCount);

    }

    public async Task DeleteCategory(long categoryId)
    {
        var cate = await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.CategoryId == categoryId && !c.IsDeleted);
        if (cate == null) throw new AppException("Category not found", (int)HttpStatusCode.BadRequest);
        cate.IsDeleted = true;
        _dbContext.Entry(cate).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<CategoryDto> GetListCategoryDtosQuery() =>
        _dbContext.Set<Category>()
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                Name = c.Name,
                CategoryId = c.CategoryId,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId,
                IsPublished = c.IsPublished,
                DisplayOrder = c.DisplayOrder,
                IncludeInMenu = c.IncludeInMenu,
                Slug = c.Slug
            });
}