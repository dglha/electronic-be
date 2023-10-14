using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Catalog;

public class ProductAttributeGroup : BaseEntity
{
    public int ProductAttributeGroupId { get; set; }
    public string Name { get; set; }
}