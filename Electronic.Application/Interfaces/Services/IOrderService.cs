using Electronic.Application.Contracts.DTOs.Order;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IOrderService
{
    Task CreateOrder();

    Task<OrderDto> GetOrderDetail(long orderId);

    Task<Pagination<OrderListDto>> GetOrders(int pageIndex, int itemPerPage);
}