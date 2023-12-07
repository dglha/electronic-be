using System.Text.Json;
using Electric.Payment.VNPay.Service;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Models.Inventory;
using Electronic.Domain.Models.Order;
using Electronic.Domain.Models.Payment;
using Electronic.Domain.Models.ShoppingCart;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class PaymentService : IPaymentService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IVnPayPaymentService _vnPayPaymentService;
    private readonly IUserService _userService;
    private readonly IAppLogger<PaymentService> _logger;

    public PaymentService(ElectronicDatabaseContext dbContext, IVnPayPaymentService vnPayPaymentService,
        IUserService userService, IAppLogger<PaymentService> logger)
    {
        _dbContext = dbContext;
        _vnPayPaymentService = vnPayPaymentService;
        _userService = userService;
        _logger = logger;
    }


    public async Task<string> CreatePaymentLink(long orderId)
    {
        var order = await _dbContext.Set<Order>()
            .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == _userService.UserId && o.OrderStatus == OrderStatusEnum.New);

        if (order is null) throw new AppException("Order not found", 400);

        return _vnPayPaymentService.CreatePaymentLink(order);
    }

    public async Task<PaymentResponseDto> VNPayPaymentCallback(object model)
    {
        var result = _vnPayPaymentService.CheckCallback(model);
        
        // Update order status
        if (!result.IsSuccess) return result;
        
        await _dbContext.Database.BeginTransactionAsync();
        

        var order = await _dbContext.Set<Order>()
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o =>
                o.OrderId == long.Parse(result.OrderId) && o.CustomerId == _userService.UserId &&
                o.OrderStatus == OrderStatusEnum.New);
        
        if (order == null) throw new AppException($"Order id {result.OrderId} not found!", 404);
        
        var orderHistory = new OrderStatusHistory
        {
            Order = order,
            OldStatus = order.OrderStatus,    
            NewStatus = OrderStatusEnum.Completed,
            Note = "Payment using VNPay"
        };
        
        order.OrderStatus = OrderStatusEnum.Completed;
        
        var payment = new Payment
        {
            CustomerId = _userService.UserId,
            Order = order,
            Amount = order.OrderTotal,
            Status = PaymentStatusEnum.Succeeded,
            FailureMessage = result.TransactionStatus,
            GatewayTransactionId = result.TransactionId,
            PaymentFee = 0
        };
        
        foreach (var orderItem in order.OrderItems)
        {
            var product = orderItem.Product;
            product.StockQuantity -= orderItem.Quantity;
            
            // Update stock history
            var stock = await _dbContext.Set<Stock>().FirstOrDefaultAsync(s => s.ProductId == product.ProductId);
            var stockHistory = new StockHistory
            {
                Note = StockHistoryNoteEnum.Sold.ToString(),
                AdjustedQuantity = -orderItem.Quantity,
                OldQuantity = product.StockQuantity.Value,
                Stock = stock,
            };
            stock.Quantity = (int)product.StockQuantity;
        
            _dbContext.Set<StockHistory>().Add(stockHistory);
        }
        
        _dbContext.Set<OrderStatusHistory>().Add(orderHistory);
        _dbContext.Set<Payment>().Add(payment);
        
        var cart = await _dbContext.Set<Cart>().FirstAsync(c => c.CustomerId == _userService.UserId);
        cart.CartItems = JsonSerializer.Serialize(new List<CartItem>());
        
        await _dbContext.SaveChangesAsync();

        await _dbContext.Database.CommitTransactionAsync();

        return result;
    }

    public async Task<Pagination<PaymentDto>> GetListPayment(int pageIndex, int itemPerPage)
    {
        var query = _dbContext.Set<Payment>().AsQueryable();

        var totalCount = await query.CountAsync();

        var data = await query.OrderByDescending(p => p.CreatedAt).Skip((pageIndex - 1) * itemPerPage).Take(itemPerPage)
            .Select(p => new PaymentDto
            {
                Amount = p.Amount,
                FailureMessage = p.FailureMessage,
                PaymentFee = p.PaymentFee,
                Status = p.Status.ToString(),
                Customer = _userService.UserId,
                GatewayTransactionId = p.GatewayTransactionId,
            }).ToListAsync();

        return Pagination<PaymentDto>.ToPagination(data, pageIndex, itemPerPage, totalCount);

    }
}