namespace Electronic.Application.Contracts.DTOs.Category;

public class CreateCategoryResultDto : CreateCategoryDto
{
    public string Slug { get; set; }
    public long CategoryId { get; set; }
    public bool IsDeleted { get; set; }
    public string ThumbnailImageUrl { get; set; }
}