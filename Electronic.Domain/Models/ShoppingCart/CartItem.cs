using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.ShoppingCart;

public class CartItem
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
    public bool IsAvailable { get; set; }
    public string? Note { get; set; }
}