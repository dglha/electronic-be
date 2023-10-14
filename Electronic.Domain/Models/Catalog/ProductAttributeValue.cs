using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Catalog;

public class ProductAttributeValue : BaseEntity
{
    public int ProductAttributeValueId { get; set; }
    public int ProductAttributeId { get; set; }
    public ProductAttribute ProductAttribute { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public string Value { get; set; }
}