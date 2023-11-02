using Electronic.Application.Contracts.DTOs.ShoppingCart;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IShoppingCartService
{
    public Task<BaseResponse<CartDto>> UpdateCart(UpdateCartDto request);
    public Task<BaseResponse<CartDto>?> GetCart();
    public Task AddToCart(long productId);
}