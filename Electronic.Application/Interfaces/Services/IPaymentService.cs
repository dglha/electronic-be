using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.Response;
using Electronic.Domain.Models.Order;
using Electronic.Domain.Models.Payment;

namespace Electronic.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<string> CreatePaymentLink(long orderId);

    Task<PaymentResponseDto> VNPayPaymentCallback(object model);

    Task<Pagination<PaymentDto>> GetListPayment(int pageIndex, int itemPerPage);
}