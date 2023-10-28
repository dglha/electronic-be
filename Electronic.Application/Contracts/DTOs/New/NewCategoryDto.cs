namespace Electronic.Application.Contracts.DTOs.New;

public class NewCategoryDto
{
    public int NewCategoryId { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public string Description { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool IsPublished { get; set; }
}