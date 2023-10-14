using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.ShoppingCart;

public class Cart : BaseEntity
{
    public long CartId { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public long CustomerId { get; set; }
}