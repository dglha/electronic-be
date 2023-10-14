using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Catalog;

public class ProductAttribute : BaseEntity
{
    public int ProductAttributeId { get; set; }
    public string Name { get; set; }
    public int ProductAttributeGroupId { get; set; }
    public ProductAttributeGroup ProductAttributeGroup { get; set; }
}