namespace Electronic.Application.Contracts.DTOs.ShoppingCart;

public class UpdateCartDto
{
    public string ClientSecret { get; set; }
    public IEnumerable<UpdateCartItemDto> CartItems { get; set; }
}