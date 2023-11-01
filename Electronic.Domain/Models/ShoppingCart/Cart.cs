using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.ShoppingCart;

public class Cart : BaseEntity
{
    public long CartId { get; set; }
    public string CustomerId { get; set; }
    public string CartItems { get; set; }
    public string? ClientSecret { get; set; }
}