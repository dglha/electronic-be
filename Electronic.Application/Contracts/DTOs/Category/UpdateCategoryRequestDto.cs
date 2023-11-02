namespace Electronic.Application.Contracts.DTOs.Category;

public class UpdateCategoryRequestDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsPublished { get; set; }
}