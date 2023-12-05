using Electronic.Application.Contracts.Common;

namespace Electronic.Application.Contracts.DTOs.New;

public class CreateNewItemDto
{
    public string Title { get; set; }
    public string ShortContent { get; set; }
    public string FullContent { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }
    public ImageFileDto ThumbnailImage { get; set; }
    public IEnumerable<int> NewCatetoryIds { get; set; }
}