namespace Electronic.Application.Interfaces.Services;

public interface IStorageService
{
    string GetMediaUrl(string fileName);
    Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null);
    Task DeleteMediaAsync(string fileName);
}