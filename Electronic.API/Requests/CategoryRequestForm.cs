using Electronic.Application.Contracts.DTOs.Category;

namespace Electronic.API.Requests;

public class CategoryRequestForm : CreateCategoryDto
{
    public IFormFile ThumbnailImage { get; set; }
}