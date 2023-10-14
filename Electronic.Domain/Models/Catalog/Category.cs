using Electronic.Domain.Common;
using Electronic.Domain.Models.Core;

namespace Electronic.Domain.Models.Catalog;

public class Category : BaseEntity
{
    public long CategoryId { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }
    
    public bool IncludeInMenu { get; set; }
    
    public long? ParentCategoryId { get; set; }
    
    public Category? ParentCategory { get; set; }

    public ICollection<Category> ChildCategories { get; protected set; } = new List<Category>();
    
    public long? ThumbnailImageId { get; set; }

    public Media? ThumbnailImage { get; set; }

    public ICollection<ProductCategory> Products { get; set; } = new List<ProductCategory>();

}