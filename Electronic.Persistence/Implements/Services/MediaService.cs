using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Models.Core;
using Electronic.Persistence.DatabaseContext;

namespace Electronic.Persistence.Implements.Services;

public class MediaService : IMediaService
{
    private readonly IStorageService _storageService;
    private readonly ElectronicDatabaseContext _dbContext;

    public MediaService(IStorageService storageService, ElectronicDatabaseContext dbContext)
    {
        _storageService = storageService;
        _dbContext = dbContext;
    }


    public string GetMediaUrl(Media? media)
    {
        return GetMediaUrl(media == null ? "no-image.png" : media.FileName);
    }

    public string GetMediaUrl(string fileName)
    {
        return _storageService.GetMediaUrl(fileName);
    }

    public string GetThumbnailUrl(Media media)
    {
        return GetMediaUrl(media);
    }

    public async Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
    {
        var media = new Media
        {
            FileName = fileName,
            MediaType = MediaTypeEnum.Image,
            Caption = "asdfadsf"
        };
        await _dbContext.Media.AddAsync(media);
        await _dbContext.SaveChangesAsync();
        await _storageService.SaveMediaAsync(mediaBinaryStream, fileName, mimeType);
    }

    public Task DeleteMediaAsync(Media media)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMediaAsync(string fileName)
    {
        return _storageService.DeleteMediaAsync(fileName);
    }
}