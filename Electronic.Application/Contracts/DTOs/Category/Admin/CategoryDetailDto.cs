namespace Electronic.Application.Contracts.DTOs.Category.Admin;

public class CategoryDetailDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public bool IncludeInMenu { get; set; }
}