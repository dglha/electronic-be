using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Catalog;

public class ProductCategory
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long CategoryId { get; set; }
    public Category Category { get; set; }
}