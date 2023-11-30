using System.Net;
using Electric.Payment.VNPay.Config;
using Electric.Payment.VNPay.DTOs.Response;
using Electric.Payment.VNPay.Helper;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.Identity;
using Electronic.Domain.Models.Order;
using Microsoft.Extensions.Options;

namespace Electric.Payment.VNPay.Service;

public class VnPayPaymentService : IVnPayPaymentService
{
    private readonly VnPayConfig _vnPayConfig;
    private readonly IUserService _userService;

    public VnPayPaymentService(IOptions<VnPayConfig> vnPayConfig, IUserService userService)
    {
        _userService = userService;
        _vnPayConfig = vnPayConfig.Value;
    }

    public string  CreatePaymentLink(Order order)
    {
        var paymentUrl = string.Empty;

        var vnPayRequest = new VnPayRequestDto(_vnPayConfig.Version,
            _vnPayConfig.TmnCode, DateTime.Now,
           _userService.IpAddress,
            order.OrderTotal, "VND",
            "120000", $"Electronic Thanh toan hoa don ${order.OrderId} ${order.OrderTotal} VND" ?? string.Empty, _vnPayConfig.ReturnUrl,
            $"{order.OrderId}");

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