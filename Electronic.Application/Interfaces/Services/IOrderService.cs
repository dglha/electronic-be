using Electronic.Application.Contracts.DTOs.Address;
using Electronic.Application.Contracts.DTOs.Order;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IOrderService
{
    Task<long> CreateOrder();

    Task<BaseResponse<OrderDto>> GetOrderDetail(long orderId);

    Task<Pagination<OrderListDto>> GetOrders(int pageIndex, int itemPerPage);
    Task<Pagination<OrderListDto>> GetOrdersByUser(int pageIndex, int itemPerPage);
    Task UpdateOrderAddress(OrderAddressDto request);
    Task<BaseResponse<List<AddressDto>>> GetUserAddresses();
}