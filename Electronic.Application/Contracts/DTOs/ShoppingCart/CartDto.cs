namespace Electronic.Application.Contracts.DTOs.ShoppingCart;

public class CartDto
{
    public IEnumerable<CartItemDto> CartItems { get; set; }
    public decimal Total { get; set; }
}