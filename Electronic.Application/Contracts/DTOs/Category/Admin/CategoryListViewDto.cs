namespace Electronic.Application.Contracts.DTOs.Category.Admin;

public class CategoryListViewDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public string Slug { get; set; }
    public ICollection<CategoryListViewDto>? Children { get; set; }
}