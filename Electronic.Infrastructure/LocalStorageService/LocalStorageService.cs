using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.LocalStorageService;

public class LocalStorageService : IStorageService
{
    private const string MediaRootFolder = "user-content";

    private readonly IWebHostEnvironment _webHostEnvironment;

    public LocalStorageService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public string GetMediaUrl(string fileName)
    {
        return $"/{MediaRootFolder}/{fileName}";
    }

    public async Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
    {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, MediaRootFolder, fileName);
        await using var output = new FileStream(filePath, FileMode.Create);
        await mediaBinaryStream.CopyToAsync(output);
    }

    public async Task DeleteMediaAsync(string fileName)
    {
        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, MediaRootFolder, fileName);
        if (File.Exists(filePath))
        {
            await Task.Run(() => File.Delete(filePath));
        }
    }
}