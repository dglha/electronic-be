using Electric.Payment.VNPay.Service;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Order;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class PaymentService : IPaymentService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IVnPayPaymentService _vnPayPaymentService;
    private readonly IUserService _userService;
    private readonly IAppLogger<PaymentService> _logger;

    public PaymentService(ElectronicDatabaseContext dbContext, IVnPayPaymentService vnPayPaymentService, IUserService userService, IAppLogger<PaymentService> logger)
    {
        _dbContext = dbContext;
        _vnPayPaymentService = vnPayPaymentService;
        _userService = userService;
        _logger = logger;
    }


    public async Task<string> CreatePaymentLink(long orderId)
    {
        var order = await _dbContext.Set<Order>().FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == _userService.UserId);

        if (order is null) throw new AppException("Order not found", 400);

        return _vnPayPaymentService.CreatePaymentLink(order);
    }

    public PaymentResponseDto VNPayPaymentCallback(object model)
    {
        return _vnPayPaymentService.CheckCallback(model);
    }
}