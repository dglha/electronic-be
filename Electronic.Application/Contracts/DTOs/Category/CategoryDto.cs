namespace Electronic.Application.Contracts.DTOs.Category;

public class CategoryDto : CreateCategoryDto
{
    public long CategoryId { get; set; }
    public string Slug { get; set; }
}