using Electronic.Application.Contracts.DTOs.Advertisement;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models;
using Electronic.Domain.Models.Core;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class AdvertisementService : IAdvertisementService
{
    private readonly IMediaService _mediaService;
    private readonly ElectronicDatabaseContext _dbContext;

    public AdvertisementService(IMediaService mediaService, ElectronicDatabaseContext dbContext)
    {
        _mediaService = mediaService;
        _dbContext = dbContext;
    }

    public async Task<List<AdvertisementDto>> GetAd()
    {
        var result = await _dbContext.Set<Advertisement>().Where(a => !a.IsDeleted).Select(a => new AdvertisementDto
        {
            Name = a.Name,
            ImageUrl = _mediaService.GetMediaUrl(a.Media),
            DisplayOrder = a.DisplayOrder
        }).ToListAsync();
        return result;
    }

    public async Task AddNewAdvertisement(CreateAdvertisementDto request)
    {
        var oldAd = await _dbContext.Set<Advertisement>().Where(a => request.DisplayOrder == a.DisplayOrder)
            .FirstOrDefaultAsync();
        if (oldAd != null)
        {
            oldAd.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }

        var ad = new Advertisement
        {
            Name = request.Name,
            DisplayOrder = request.DisplayOrder,
            Description = request.Description,
            SortIndex = 0,
            IsPublished = true,
        };

        await SaveAdImage(ad, request.Image.FileContent, request.Image.FileName, request.Image.FileType);
        await _dbContext.Set<Advertisement>().AddAsync(ad);

        await _dbContext.SaveChangesAsync();
    }
    
    private async Task<string> SaveFile(Stream mediaBinaryStream, string fileName, string mimeType)
    {
        var originalFileName = fileName.Trim('"');
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _mediaService.SaveMediaAsync(mediaBinaryStream, newFileName, mimeType);
        return newFileName;
    }
    
    private async Task SaveAdImage(Advertisement ad, Stream mediaBinaryStream, string fileName,
        string mimeType)
    {
        var name = await SaveFile(mediaBinaryStream, fileName, mimeType);
        if (ad.Media != null)
        {
            ad.Media.FileName = name;
        }
        else
        {
            ad.Media = new Media { FileName = name, Caption = ""};
        }
    }
}