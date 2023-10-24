using Electronic.Domain.Models.Core;

namespace Electronic.Application.Interfaces.Services;

public interface IMediaService
{
    string GetMediaUrl(Media? media);

    string GetMediaUrl(string fileName);

    string GetThumbnailUrl(Media media);

    Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null);

    Task DeleteMediaAsync(Media media);

    Task DeleteMediaAsync(string fileName);
}