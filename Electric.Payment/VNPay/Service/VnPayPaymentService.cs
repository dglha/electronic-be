using Electric.Payment.VNPay.Config;
using Electric.Payment.VNPay.DTOs.Response;
using Electric.Payment.VNPay.Helper;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Domain.Models.Order;
using Microsoft.Extensions.Options;

namespace Electric.Payment.VNPay.Service;

public class VnPayPaymentService : IVnPayPaymentService
{
    private readonly VnPayConfig _vnPayConfig;

    public VnPayPaymentService(IOptions<VnPayConfig> vnPayConfig)
    {
        _vnPayConfig = vnPayConfig.Value;
    }

    public string CreatePaymentLink(Order order)
    {
        var paymentUrl = string.Empty;

        var vnPayRequest = new VnPayRequestDto(_vnPayConfig.Version,
            _vnPayConfig.TmnCode, DateTime.Now, "192.168.1.1" ?? string.Empty,
            order.OrderTotal, "VND" ?? string.Empty,
            "120000", $"Electronic Thanh toan ${order.OrderId} ${order.OrderTotal} VND" ?? string.Empty, _vnPayConfig.ReturnUrl,
            Guid.NewGuid().ToString());

        paymentUrl = vnPayRequest.GetLink(_vnPayConfig.PaymentUrl, _vnPayConfig.HashSecret);
        return paymentUrl;
    }

    public PaymentResponseDto CheckCallback(object model)
    {
        var response = model as VnPayResponseDto;

        var isValidSignature = response.IsValidSignature(_vnPayConfig.HashSecret);

        if (!isValidSignature)
        {
            return new PaymentResponseDto
            {
                IsSuccess = false,
                VnPayResponseCode = response.vnp_ResponseCode
            };
        }

        return new PaymentResponseDto
        {
            IsSuccess = true,
            VnPayResponseCode = response.vnp_ResponseCode,
            OrderId = response.vnp_TxnRef,
            OrderDescription = response.vnp_OrderInfo,
            TransactionId = response.vnp_TransactionNo,
            TransactionStatus = response.vnp_TransactionStatus
        };
    }
}