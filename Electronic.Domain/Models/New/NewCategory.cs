using Electronic.Domain.Common;

namespace Electronic.Domain.Models.New;

public class NewCategory : BaseEntity
{
    public NewCategory()
    {
        
    }
    
    public int NewCategoryId { get; set; }

    public string Name { get; set; }
    
    public string Slug { get; set; }
    
    public string Description { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool IsPublished { get; set; }
    
    public bool IsDeleted { get; set; }

    public ICollection<NewItemNewCategory> NewItemNewCategories { get; set; } = new List<NewItemNewCategory>();

}