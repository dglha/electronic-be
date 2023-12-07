using Electric.Payment.VNPay.DTOs.Response;
using Electric.Payment.VNPay.Service;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IPaymentService _paymentService;
        private readonly IVnPayPaymentService _vnPayPaymentService;

        public PaymentController(IPaymentService paymentService, IVnPayPaymentService vnPayPaymentService)
        {
            _paymentService = paymentService;
            _vnPayPaymentService = vnPayPaymentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> GetPaymentLink([FromBody] long orderId)
        {
            return Ok(await _paymentService.CreatePaymentLink(orderId));
        }
        
        [HttpGet]
        [Route("vnpay-return")]
        public async Task<ActionResult<PaymentResponseDto>> VnpayReturn([FromQuery]VnPayResponseDto response)
        {
            return Ok(await  _paymentService.VNPayPaymentCallback(response));
        }
        
        
    }
}
