using System.Text.Json;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Order;
using Electronic.Domain.Models.ShoppingCart;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class OrderService : IOrderService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<OrderService> _logger;
    private readonly IUserService _userService;

    public OrderService(ElectronicDatabaseContext dbContext, IAppLogger<OrderService> logger, IUserService userService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
    }

    public async Task CreateOrder()
    {
        var cart = await _dbContext.Set<Cart>()
            .Where(c => c.CustomerId == _userService.UserId).FirstOrDefaultAsync();

        if (cart == null) throw new AppException("Cannot make order right now", 400);

        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart.CartItems);
        
        if (cartItems == null) throw new AppException("Cannot make order right now, your cart is empty1", 400);
        var validProductIdList = await GetValidProductIdList(cart);
        
        if (validProductIdList.Count == 0 ) throw new AppException("Cannot make order right now, your cart is empty", 400);

        var orderItems = new List<OrderItem>();

        var products = await _dbContext.Set<Product>().Where(p => validProductIdList.Contains(p.ProductId)).ToListAsync();

        await _dbContext.Database.BeginTransactionAsync();
        try
        {
            foreach (var product in products)
            {
                var price = product.SpecialPriceEndDate < DateTime.Now ? product.Price : product.SpecialPrice;
                var discountAmount = product.SpecialPriceEndDate < DateTime.Now ? 0 : (product.Price - product.SpecialPrice);
                var quantity = cartItems.First(c => c.ProductId == product.ProductId).Quantity;
                var orderItem = new OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    ProductPrice = price,
                    DiscountAmount = discountAmount,
                    TaxAmount = price * quantity * 10 / 100,
                    TaxPercent = 10m
                };
            
                orderItems.Add(orderItem);            
            }

            var order = new Order
            {
                CustomerId = _userService.UserId,
                TaxAmount = orderItems.Sum(o => o.TaxAmount),
                OrderTotal = orderItems.Sum(o => o.TaxAmount + o.ProductPrice * o.Quantity),
                OrderStatus = OrderStatusEnum.New,
                ShippingMethod = "Test",
                OrderItems = orderItems
            };

            var orderStatusHistory = new OrderStatusHistory
            {
                Order = order,
                Note = "Create new order",
                NewStatus = OrderStatusEnum.New,
            };

            _dbContext.Set<Order>().Add(order);
            _dbContext.Set<OrderItem>().AddRange(orderItems);
            _dbContext.Set<OrderStatusHistory>().Add(orderStatusHistory);

            await _dbContext.SaveChangesAsync();
            
            await _dbContext.Database.CommitTransactionAsync();
        }
        
        catch (Exception e)
        {
            throw new AppException(e.Message, 500);
        }
    }

    private async Task<List<long>> GetValidProductIdList(Cart cart)
    {
        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart.CartItems);
        
        if (cartItems.Count == 0) return new List<long> { };
        
        var cartItemProductIds = cartItems.Select(ci => ci.ProductId).ToList();
        
        var products = await _dbContext.Set<Product>().Where(p => cartItemProductIds.Contains(p.ProductId))
            .ToListAsync();

        return products.Select(p => p.ProductId).ToList();
    }
    
}