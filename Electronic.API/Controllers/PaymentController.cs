using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electric.Payment.VNPay.DTOs.Response;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> GetPaymentLink(long orderId)
        {
            return Ok(await _paymentService.CreatePaymentLink(orderId));
        }
        
        [HttpGet]
        [Route("vnpay-return")]
        public async Task<ActionResult<PaymentResponseDto>> VnpayReturn([FromQuery]VnPayResponseDto response)
        {
            return Ok( _paymentService.VNPayPaymentCallback(response));
        }
    }
}
