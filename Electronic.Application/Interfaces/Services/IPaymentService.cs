using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Domain.Models.Order;

namespace Electronic.Application.Interfaces.Services;

public interface IPaymentService
{
    Task<string> CreatePaymentLink(long orderId);

    PaymentResponseDto VNPayPaymentCallback(object model);
}