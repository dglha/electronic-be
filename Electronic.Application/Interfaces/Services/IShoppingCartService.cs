using Electronic.Application.Contracts.DTOs.ShoppingCart;
using Electronic.Application.Contracts.Response;
using Electronic.Domain.Models.ShoppingCart;

namespace Electronic.Application.Interfaces.Services;

public interface IShoppingCartService
{
    public Task<BaseResponse<CartDto>> UpdateCart(UpdateCartDto request);
    public Task<BaseResponse<CartDto>?> GetCart();
    public Task AddToCart(CartItem request);
    public Task CheckValidCart();
}