using System.Net;
using System.Text.Json;
using Electronic.Application.Contracts.DTOs.ShoppingCart;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.ShoppingCart;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Electronic.Persistence.Implements.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<ShoppingCartService> _logger;
    private readonly IUserService _userService;
    private readonly IMediaService _mediaService;

    public ShoppingCartService(ElectronicDatabaseContext dbContext, IAppLogger<ShoppingCartService> logger, IUserService userService, IMediaService mediaService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
        _mediaService = mediaService;
    }

    public async Task UpdateCart(UpdateCartDto request)
    {
        var isLogged = _userService.IsLogged;
        if (!isLogged && request.ClientSecret.IsNullOrEmpty())
            throw new AppException("Error when updating shopping cart", 400);

        var cart = _dbContext
            .Set<Cart>()
            .FirstOrDefault(c => isLogged ? c.CustomerId == _userService.UserId : c.ClientSecret == request.ClientSecret);

        if (!isLogged) return;  // TODO: Using redis - implement later

        if (cart == null)
        {
            var newCart = new Cart
            {
                CustomerId = _userService.UserId,
                CartItems = JsonSerializer.Serialize(request.CartItems),
            };
            _dbContext.Set<Cart>().Add(newCart);
            await _dbContext.SaveChangesAsync();

            return;
        }
        
        // Update cart
        var cartItemProductIds = request.CartItems.Select(ci => ci.ProductId).ToList();
        var products = await _dbContext.Set<Product>().Where(p => cartItemProductIds.Contains(p.ProductId)).ToListAsync();

        var validProductIds = products.Select(p => p.ProductId).ToList();
        var isValidProducts = cartItemProductIds.All(x => validProductIds.Contains(x));

        if (!isValidProducts) throw new AppException("Invalid product id", (int)HttpStatusCode.BadRequest);
        
        // Validation for cart item
        foreach( var cartItem in request.CartItems)
        {
            var product = products.First(p => p.ProductId == cartItem.ProductId);
            if (product.HasOption || !product.StockQuantity.HasValue)
                throw new AppException("Product are not allow to add to cart", (int)HttpStatusCode.BadRequest);
            if (product.StockQuantity is 0)
                throw new AppException("Not enough product in stock", (int)HttpStatusCode.BadRequest);
            if (cartItem.Quantity > product.StockQuantity)
                cartItem.Quantity = product.StockQuantity.Value;
        }
        
        cart.CartItems = JsonSerializer.Serialize(request.CartItems);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<BaseResponse<CartDto>?> GetCart()
    {
        var isLogged = _userService.IsLogged;
        if (!isLogged) return null;

        var cart = await _dbContext.Set<Cart>().Where(c => c.CustomerId == _userService.UserId).FirstOrDefaultAsync();
        if (cart == null)
            return new BaseResponse<CartDto>(
                new CartDto
                {
                    Total = 0m,
                    CartItems = new List<CartItemDto>()
                }
            );

        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart.CartItems);
        var productIds = cartItems.Select(p => p.ProductId).ToList();

        var cartItemDtos = _dbContext.Set<Product>()
            .Include(p => p.OptionCombinations)
            .Include(p => p.ThumbnailImage)
            .Where(p => productIds.Contains(p.ProductId))
            .AsEnumerable()
            .Select(p =>
            {
                var price = p.SpecialPriceEndDate < DateTime.Now ? p.Price : p.SpecialPrice;
                var quantity = cartItems.First(c => c.ProductId == p.ProductId).Quantity;
                
                return new CartItemDto
                {
                    Name = p.Name,
                    Price = price,
                    Quantity = quantity,
                    TotalPrice = price * quantity,
                    ThumbnailImageUrl = _mediaService.GetThumbnailUrl(p.ThumbnailImage),
                    Options = p.OptionCombinations.Select(ob => ob.Value).ToList()
                };
            }).ToList();

        return new BaseResponse<CartDto>(new CartDto
        {
            Total = cartItemDtos.Sum(c => c.TotalPrice),
            CartItems = cartItemDtos
        });
    }
}