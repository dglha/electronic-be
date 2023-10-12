using System.ComponentModel.DataAnnotations;
using Electronic.Domain.Common;

namespace Electronic.Domain.Model.Catalog;

public class Brand : BaseEntity
{
    public int BrandId { get; set; }
    
    public string Name { get; set; }
    
    public string Slug { get; set; }

    public string Description { get; set; }

    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }
}