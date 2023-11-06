using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Domain.Models.Order;

namespace Electric.Payment.VNPay.Service;

public interface IVnPayPaymentService
{
    public string CreatePaymentLink(Order order);
    
    public PaymentResponseDto CheckCallback(object model);
}