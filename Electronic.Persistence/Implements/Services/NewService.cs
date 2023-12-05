using System.Net;
using Electronic.Application.Contracts.DTOs.New;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Core;
using Electronic.Domain.Models.New;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class NewService : INewService
{
    private readonly INewCategoryRepository _newCategoryRepository;
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IMediaService _mediaService;

    public NewService(INewCategoryRepository newCategoryRepository, ElectronicDatabaseContext dbContext, IMediaService mediaService)
    {
        _newCategoryRepository = newCategoryRepository;
        _dbContext = dbContext;
        _mediaService = mediaService;
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

    public async Task AddNewItem(CreateNewItemDto request)
    {
        if (!request.NewCatetoryIds.Any())
            throw new AppException("Invalid Category id(s)", (int)HttpStatusCode.BadRequest);
        
        var slug = SlugGenerator.Generate(request.Title);

        var newItem = new NewItem
        {
            IsPublished = request.IsPublished,
            IsDeleted = false,
            Slug = slug,
            Title = request.Title,
            FullContent = request.FullContent,
            ShortContent = request.ShortContent,
            PublishedAt = DateTime.Now
        };

        foreach (var categoryId in request.NewCatetoryIds)
        {
            var newItemCategory = new NewItemNewCategory
            { 
                NewCategoryId = categoryId,
                NewItem = newItem,
            };
            _dbContext.Set<NewItemNewCategory>().Add(newItemCategory);
        }

        await SaveNewItemThumbnail(newItem, request.ThumbnailImage.FileContent, request.ThumbnailImage.FileName, request.ThumbnailImage.FileType);

        _dbContext.Set<NewItem>().Add(newItem);
        await _dbContext.SaveChangesAsync();
    }

    public Task RemoveNewItem()
    {
        throw new NotImplementedException();
    }

    public async Task<Pagination<NewItemDto>> GetNewItems(int pageIndex, int itemPerPage)
    {
        var query = _dbContext.Set<NewItem>().AsQueryable();
        var totalCount = await query.CountAsync();
        var data = await query.Skip((pageIndex - 1) * itemPerPage).Take(itemPerPage)
            .Select(n => new NewItemDto
            {
                NewItemId = n.NewItemId,
                IsPublished = n.IsPublished,
                IsDeleted = n.IsPublished,
                Slug = n.Slug,
                Title = n.Title,
                // FullContent = n.FullContent,
                ShortContent = n.ShortContent,
                ThumbnailImageUrl = _mediaService.GetThumbnailUrl(n.ThumbnailImage),
                PublishedAt = n.PublishedAt
            }).ToListAsync();
        return Pagination<NewItemDto>.ToPagination(data, pageIndex, itemPerPage, totalCount);
    }

    public async Task<BaseResponse<ICollection<NewCategoryDto>>> GetNewCategories()
    {
        var categories = await _dbContext.Set<NewCategory>().Select(c => new NewCategoryDto
        {
            Name = c.Name,
            NewCategoryId = c.NewCategoryId,
            Description = c.Description,
            IsPublished = c.IsPublished,
            Slug = c.Slug,
        }).ToListAsync();

        return new BaseResponse<ICollection<NewCategoryDto>>(categories);
    }

    public async Task<BaseResponse<NewItemDetailDto>> GetNewItemDetails(int newItemId)
    {
        var newItem = await _dbContext.Set<NewItem>().FirstOrDefaultAsync(n => n.NewItemId == newItemId && !n.IsDeleted);

        if (newItem is null) throw new AppException("New item not found!", (int)HttpStatusCode.BadRequest);

        var newDto = new NewItemDetailDto
        {
            Slug = newItem.Slug,
            Title = newItem.Title,
            FullContent = newItem.FullContent,
            ShortContent = newItem.ShortContent,
            ThumbnailImageUrl = _mediaService.GetThumbnailUrl(newItem.ThumbnailImage),
            PublishedAt = newItem.PublishedAt
        };

        return new BaseResponse<NewItemDetailDto>(newDto);
    }

    public async Task<Pagination<NewItemDto>> GetUserNewItems(int pageIndex, int itemPerPage, int categoryId = 0)
    {
        var query = _dbContext.Set<NewItem>().Where(n => !n.IsDeleted && n.IsPublished).AsQueryable();

        if (categoryId != 0)
        {
            query = query.Where(c => c.NewItemNewCategories.Any(n => n.NewCategoryId == categoryId));
        }
        var totalCount = await query.CountAsync();
        var data = await query.Skip((pageIndex - 1) * itemPerPage).Take(itemPerPage)
            .Select(n => new NewItemDto
            {
                NewItemId = n.NewItemId,
                IsPublished = n.IsPublished,
                IsDeleted = n.IsPublished,
                Slug = n.Slug,
                Title = n.Title,
                // FullContent = n.FullContent,
                ShortContent = n.ShortContent,
                ThumbnailImageUrl = _mediaService.GetThumbnailUrl(n.ThumbnailImage),
                PublishedAt = n.PublishedAt
            }).OrderByDescending(n => n.PublishedAt).ToListAsync();
        return Pagination<NewItemDto>.ToPagination(data, pageIndex, itemPerPage, totalCount);
    }

    public async Task<BaseResponse<ICollection<NewCategoryDto>>> GetUserNewCategories()
    {
        var categories = await _dbContext.Set<NewCategory>().Where(c => !c.IsDeleted && c.IsPublished).Select(c => new NewCategoryDto
        {
            Name = c.Name,
            NewCategoryId = c.NewCategoryId,
            Description = c.Description,
            IsPublished = c.IsPublished,
            Slug = c.Slug,
        }).ToListAsync();

        return new BaseResponse<ICollection<NewCategoryDto>>(categories);
    }

    private async Task SaveNewItemThumbnail(NewItem newItem, Stream mediaBinaryStream, string fileName,
        string mimeType)
    {
        var name = await SaveFile(mediaBinaryStream, fileName, mimeType);
        if (newItem.ThumbnailImage != null)
        {
            newItem.ThumbnailImage.FileName = name;
        }
        else
        {
            newItem.ThumbnailImage = new Media { FileName = name, Caption = ""};
        }
    }
    
    private async Task<string> SaveFile(Stream mediaBinaryStream, string fileName, string mimeType)
    {
        var originalFileName = fileName.Trim('"');
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _mediaService.SaveMediaAsync(mediaBinaryStream, newFileName, mimeType);
        return newFileName;
    }
}