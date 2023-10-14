using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Core;

namespace Electronic.Domain.Models.Catalog;

public class ProductMedia : BaseEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long MediaId { get; set; }
    public Media Media { get; set; }
    public int DisplayOrder { get; set; }
}