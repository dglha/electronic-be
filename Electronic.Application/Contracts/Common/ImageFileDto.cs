namespace Electronic.Application.Contracts.Common;

public class ImageFileDto
{
    public Stream FileContent { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
}