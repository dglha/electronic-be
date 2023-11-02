﻿using System.Net;
using Electronic.Application.Contracts.DTOs.Category;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Persistences;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class CategoryService : ICategoryService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<CategoryService> _logger;
    private readonly ICategoryRepository _repository;
    private readonly IMediaService _mediaService;

    public CategoryService(ElectronicDatabaseContext dbContext, IAppLogger<CategoryService> logger,
        ICategoryRepository repository, IMediaService mediaService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _repository = repository;
        _mediaService = mediaService;
    }

    public async Task<CreateCategoryResultDto> CreateNewCategory(CreateCategoryDto request, Stream mediaBinaryStream, string fileName = null, string mimeType = null)
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

        await SaveCategoryImage(category, mediaBinaryStream, fileName, mimeType);
        await _repository.CreateAsync(category);
        
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
            IncludeInMenu = category.IncludeInMenu,
            ThumbnailImageUrl = _mediaService.GetThumbnailUrl(category.ThumbnailImage)
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

    public async Task UpdateCategoryChildren(long categoryId, List<long> validChildIds)
    {
        var parentCategory = await _dbContext.Set<Category>().FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        
        if (parentCategory == null) throw new AppException("Category not found", 404);
        
        var oldChildCategory = await _dbContext.Set<Category>()
            .Where(c => c.ParentCategoryId.HasValue && c.ParentCategoryId.Value == parentCategory.CategoryId).ToListAsync();
        
        foreach (var removeCate in oldChildCategory.Where(c => !validChildIds.Contains(c.CategoryId)).ToList())
        {
            removeCate.ParentCategoryId = null;
        }
        
        var childrenCategory = await _dbContext.Set<Category>()
            .Where(c => validChildIds.Contains(c.CategoryId) && !c.ParentCategoryId.HasValue).ToListAsync();

        foreach (var cate in childrenCategory)
        {
            if (await HaveCircularNesting(cate.CategoryId, parentCategory.CategoryId))
                throw new AppException("Parent category cannot be itself children", 400);
            cate.ParentCategory = parentCategory;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<CategoryDto>> GetListAvailableCategory()
    {
        return await _dbContext.Set<Category>()
            .Where(c => !c.IsDeleted && !c.ParentCategoryId.HasValue)
            .Select(c => new CategoryDto
            {
                Name = c.Name,
                CategoryId = c.CategoryId,
            }).ToListAsync();
    }

    public async Task<BaseResponse<CategoryDto>> UpdateCategory(long categoryid, UpdateCategoryRequestDto request)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == categoryid);
        
        if (category == null) throw new AppException("Category not found", 404);

        category.Name = request.Name;
        category.IsPublished = request.IsPublished;
        category.Description = request.Description;

        await _dbContext.SaveChangesAsync();

        return new BaseResponse<CategoryDto>(new CategoryDto
        {
            Name = category.Name,
            CategoryId = category.CategoryId,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            IsPublished = category.IsPublished,
            DisplayOrder = category.DisplayOrder,
            IncludeInMenu = category.IncludeInMenu,
            Slug = category.Slug
        });
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

    private async Task SaveCategoryImage(Category category, Stream mediaBinaryStream, string fileName,
        string mimeType)
    {
        var name = await SaveFile(mediaBinaryStream, fileName, mimeType);
        if (category.ThumbnailImage != null)
        {
            category.ThumbnailImage.FileName = name;
        }
        else
        {
            category.ThumbnailImage = new Media { FileName = name, Caption = ""};
        }
    }

    private async Task<string> SaveFile(Stream mediaBinaryStream, string fileName, string mimeType)
    {
        var originalFileName = fileName.Trim('"');
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _mediaService.SaveMediaAsync(mediaBinaryStream, newFileName, mimeType);
        return newFileName;
    }
    
    private async Task<bool> HaveCircularNesting(long childId, long parentId)
    {
        var category = await _dbContext.Set<Category>().FirstOrDefaultAsync(x => x.CategoryId == parentId);
        var parentCategoryId = category.ParentCategoryId;
        while (parentCategoryId.HasValue)
        {
            if(parentCategoryId.Value == childId)
            {
                return true;
            }

            var parentCategory = await _dbContext.Set<Category>().FirstAsync(x => x.CategoryId == parentCategoryId);
            parentCategoryId = parentCategory.ParentCategoryId;
        }

        return false;
    }
}