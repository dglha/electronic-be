using Electronic.Application.Contracts.DTOs.ShoppingCart;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IShoppingCartService
{
    public Task UpdateCart(UpdateCartDto request);
    public Task<BaseResponse<CartDto>?> GetCart();
}