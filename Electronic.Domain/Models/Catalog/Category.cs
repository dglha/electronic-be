using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Electronic.Domain.Common;
using Electronic.Domain.Models.Core;

namespace Electronic.Domain.Model.Catalog;

public class Category : BaseEntity
{
    public long CategoryId { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IncludeInMenu { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public long? ParentCategoryId { get; set; }
    
    public Category? ParentCategory { get; set; }

    public virtual ICollection<Category> ChildCategories { get; protected set; } = new List<Category>();
    
    public long ThumbnailImageId { get; set; }

    public Media ThumbnailImage { get; set; }
}