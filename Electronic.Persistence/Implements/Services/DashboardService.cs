using Electronic.Application.Contracts.DTOs.Dashboard;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Order;
using Electronic.Domain.Models.Payment;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class DashboardService : IDashboardService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IPaymentService _paymentService;

    public DashboardService(ElectronicDatabaseContext dbContext, IPaymentService paymentService)
    {
        _dbContext = dbContext;
        _paymentService = paymentService;
    }


    public async Task<BaseResponse<DashboardDto>> GetDashboarData()
    {
        var orderQuery = _dbContext.Set<Order>()
            .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == DateTime.Today.Month);
        var todayOrder = await orderQuery.CountAsync();

        var todayCompletedOrder = await orderQuery.Where(o => o.OrderStatus == OrderStatusEnum.Completed).CountAsync();

        var totalIncome = await _dbContext.Set<Payment>().Where(p =>
            p.Status == PaymentStatusEnum.Succeeded && p.CreatedAt.HasValue &&
            p.CreatedAt.Value.Month == DateTime.Today.Month).SumAsync(p => p.Amount);

        var totalSold = await orderQuery.Where(o => o.OrderStatus == OrderStatusEnum.Completed)
            .SelectMany(o => o.OrderItems.Select(i => i.Quantity)).SumAsync();

        // var topSoldProductIds = await orderQuery.Where(o => o.OrderStatus == OrderStatusEnum.Completed)
        //     .SelectMany(o => o.OrderItems.Select(i => i.ProductId)).ToListAsync();

        var topSoldProductIds = await (from orderItems in _dbContext.OrderItems.AsNoTracking()
            join orders in orderQuery on orderItems.OrderId equals orders.OrderId
            where orders.OrderStatus == OrderStatusEnum.Completed
            group orderItems by orderItems.ProductId
            into g
            select new
            {
                ProductId = g.Key,
                TotalSold = g.Count()
            }).OrderByDescending(p => p.TotalSold).Take(5).Select(p => p.ProductId).ToListAsync();
                

        var topSoldProduct = await _dbContext.Set<Product>().Where(p => topSoldProductIds.Contains(p.ProductId)).Select(
            p => new ProductListDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
            }).ToListAsync();

        return new BaseResponse<DashboardDto>(new DashboardDto
        {
            TodayOrder = todayOrder,
            TodayCompletedOrder = todayCompletedOrder,
            TotalIncome = totalIncome,
            TotalSold = totalSold,
            TopSoldProduct = topSoldProduct
        });
    }

    public async Task<Pagination<PaymentDto>> GetLatestPayment()
    {
        return await _paymentService.GetListPayment(1, 10);
    }
}