namespace Electronic.Application.Contracts.DTOs.New;

public class NewItemDetailDto
{
    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public string ShortContent { get; set; }
    
    public string FullContent { get; set; }

    public DateTime PublishedAt { get; set; }
    
    public string ThumbnailImageUrl { get; set; }
}