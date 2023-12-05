namespace Electronic.Application.Contracts.DTOs.New;

public class NewItemDto
{
    public int NewItemId { get; set; }

    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public string? ShortContent { get; set; }
    
    public string FullContent { get; set; }
    
    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime PublishedAt { get; set; }
    
    public string ThumbnailImageUrl { get; set; }
}