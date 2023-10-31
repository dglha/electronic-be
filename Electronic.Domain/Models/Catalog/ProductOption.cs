using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Catalog;

public class ProductOption : BaseEntity
{
    public int ProductOptionId { get; set; }
    public string Name { get; set; }
}