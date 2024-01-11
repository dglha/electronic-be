using System.Net;
using System.Text.Json;
using Electronic.Application.Contracts.DTOs.Address;
using Electronic.Application.Contracts.DTOs.Order;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Core;
using Electronic.Domain.Models.Inventory;
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
    private readonly IMediaService _mediaService;
    private readonly IShoppingCartService _shoppingCartService;

    public readonly int TAX_PERCENT = 10;

    public OrderService(ElectronicDatabaseContext dbContext, IAppLogger<OrderService> logger, IUserService userService, IMediaService mediaService, IShoppingCartService shoppingCartService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
        _mediaService = mediaService;
        _shoppingCartService = shoppingCartService;
    }

    public async Task<long> CreateOrder()
    {
        var isCanCreateOrder = await _shoppingCartService.CheckValidCart();

        if (!isCanCreateOrder)
            throw new AppException("Hiện không thể tạo đơn hàng, vui lòng kiểm tra giỏ hàng của quý khách", (int)HttpStatusCode.BadRequest);
        
        var cart = await _dbContext.Set<Cart>()
            .Where(c => c.CustomerId == _userService.UserId).FirstOrDefaultAsync();

        if (cart == null) throw new AppException("Cannot make order right now", 400);

        var cartItems = JsonSerializer.Deserialize<List<CartItem>>(cart.CartItems);
        
        if (cartItems == null) throw new AppException("Cannot make order right now, your cart is empty", 400);
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
                    TaxAmount = price * quantity * TAX_PERCENT / 100,
                    TaxPercent = 10m
                };
            
                orderItems.Add(orderItem);

                // Update stock history
                var stock = await _dbContext.Set<Stock>().FirstOrDefaultAsync(s => s.ProductId == product.ProductId);
                var stockHistory = new StockHistory
                {
                    Note = "Tạo đơn đặt hàng",
                    AdjustedQuantity = -orderItem.Quantity,
                    OldQuantity = product.StockQuantity.Value,
                    Stock = stock,
                };
                product.StockQuantity -= quantity;
                stock.Quantity = (int)product.StockQuantity;
        
                _dbContext.Set<StockHistory>().Add(stockHistory);
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
                Note = "Tạo đơn đặt hàng mới",
                NewStatus = OrderStatusEnum.New,
            };

            _dbContext.Set<Order>().Add(order);
            _dbContext.Set<OrderItem>().AddRange(orderItems);
            _dbContext.Set<OrderStatusHistory>().Add(orderStatusHistory);
            
            // TODO: Clear cart
            cart.CartItems = JsonSerializer.Serialize(new List<CartItem> { });

            await _dbContext.SaveChangesAsync();
            
            await _dbContext.Database.CommitTransactionAsync();

            return order.OrderId;
        }
        
        catch (Exception e)
        {
            throw new AppException(e.Message, 500);
        }
    }

    public async Task<BaseResponse<OrderDto>> GetOrderDetail(long orderId)
    {
        var order =  await _dbContext.Set<Order>()
            .Include(o => o.OrderStatusHistories)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product).ThenInclude(product => product.OptionCombinations)
            .Include(order => order.OrderItems).ThenInclude(orderItem => orderItem.Product)
            .ThenInclude(product => product.ThumbnailImage)
            .Include(o => o.Address)
            .Where(o => o.CustomerId == _userService.UserId && o.OrderId == orderId).FirstOrDefaultAsync();

        if (order is null) throw new AppException("Order not found!", 400);

        var orderItems = order.OrderItems.Select(oi => new OrderItemDto
        {
            ProductId = oi.ProductId,
            Quantity = oi.Quantity,
            ProductPrice = oi.ProductPrice,
            Name = oi.Product.Name,
            DiscountAmount = oi.DiscountAmount,
            TaxAmount = oi.TaxAmount,
            TaxPercent = oi.TaxAmount,
            Options = oi.Product.OptionCombinations.Select(oc => oc.Value).ToList(),
            Slug = oi.Product.Slug,
            ThumbnailImageUrl = _mediaService.GetThumbnailUrl(oi.Product.ThumbnailImage),
        }).ToList();

        var orderStatusHistoriesDto = order.OrderStatusHistories.Select(x => new OrderStatusHistoryDto
        {
            Note = x.Note, NewStatus = x.NewStatus.ToString(), OldStatus = x.OldStatus.ToString()
        }).ToList();

        var result = new OrderDto
        {
            OrderId = order.OrderId,
            TaxAmount = order.TaxAmount ?? 0,
            OrderItems = orderItems,
            OrderStatus = order.OrderStatus.ToString(),
            OrderTotal = order.OrderTotal,
            ShippingMethod = order.ShippingMethod,
            OrderStatusHistories = orderStatusHistoriesDto,
            PaymentMethod = order.PaymentMethod,
            PaymentFeeAmount = order.PaymentFeeAmount ?? 0,
            Address = order.Address != null ? new OrderAddressDto
            {
                Address = order.Address.AddressLineOne,
                PaymentMethod = order.PaymentMethod,
                City = order.Address.City,
                Email = order.Address.Email,
                FirstName = order.Address.FirstName,
                Lastname = order.Address.LastName,
                PhoneNumber = order.Address.PhoneNumber,
                ZipCode = order.Address.ZipCode,
                OrderId = order.OrderId,
            } : null
        };

        return new BaseResponse<OrderDto>(result);
    }

    public async Task<Pagination<OrderListDto>> GetOrders(int pageIndex, int itemPerPage)
    {
        var query = _dbContext.Orders.OrderByDescending(o => o.UpdatedAt).AsQueryable();

        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageIndex - 1) * itemPerPage).Take(itemPerPage)
            .Select(o => new OrderListDto
            {
                OrderId = o.OrderId,
                TaxAmount = o.TaxAmount ?? 0,
                OrderStatus = o.OrderStatus.ToString(),
                OrderTotal = o.OrderTotal,
                Customer = _userService.UserEmail
            }).ToListAsync();

        return Pagination<OrderListDto>.ToPagination(data, pageIndex, itemPerPage, totalCount);
    }
    
    public async Task<Pagination<OrderListDto>> GetOrdersByUser(int pageIndex, int itemPerPage)
    {
        var query = _dbContext.Orders.Where(o => o.CustomerId == _userService.UserId).OrderByDescending(o => o.UpdatedAt).AsQueryable();

        var totalCount = await query.CountAsync();
        var data = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageIndex - 1) * itemPerPage).Take(itemPerPage)
            .Select(o => new OrderListDto
            {
                OrderId = o.OrderId,
                TaxAmount = o.TaxAmount ?? 0,
                OrderStatus = o.OrderStatus.ToString(),
                OrderTotal = o.OrderTotal,
                Customer = _userService.UserEmail
            }).ToListAsync();

        return Pagination<OrderListDto>.ToPagination(data, pageIndex, itemPerPage, totalCount);
    }

    public async Task UpdateOrderAddress(OrderAddressDto request)
    {
        var order =  await _dbContext.Set<Order>()
            .Where(o => o.CustomerId == _userService.UserId && o.OrderId == request.OrderId && o.OrderStatus == OrderStatusEnum.New).FirstOrDefaultAsync();

        if (order is null) throw new AppException("Order not found!", (int)HttpStatusCode.NotFound);

        var address = new Address
        {
            CustomerId = _userService.UserId,
            City = request.City,
            AddressLineOne = request.Address,
            AddressLineTwo = request.Address,
            PhoneNumber = request.PhoneNumber,
            ZipCode = request.ZipCode,
            FirstName = request.FirstName,
            LastName = request.Lastname,
            Email = request.Email,
            ContactName = request.FirstName + request.Lastname
        };

        order.Address = address;
        order.PaymentMethod = request.PaymentMethod;
        
        await _dbContext.Set<Address>().AddAsync(address);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<BaseResponse<List<AddressDto>>> GetUserAddresses()
    {
        var response = await _dbContext.Set<Address>().Where(a => a.CustomerId == _userService.UserId && a.IsDefault).Select(a => new AddressDto
        {
            UserId = a.CustomerId,
            FirstName = a.FirstName,
            LastName = a.LastName,
            City = a.City,
            Email = a.Email,
            Address = a.AddressLineOne,
            Zipcode = a.ZipCode,
            AddressId = a.AddressId
        }).ToListAsync();
        return new BaseResponse<List<AddressDto>>(response);
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