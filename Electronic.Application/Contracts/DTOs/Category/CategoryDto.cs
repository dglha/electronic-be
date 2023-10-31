namespace Electronic.Application.Contracts.DTOs.Category;

public class CategoryDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public bool? IncludeInMenu { get; set; }
    
    public long? ParentCategoryId { get; set; }
    public long CategoryId { get; set; }
    public string Slug { get; set; }
}