using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Order;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<long>> CreateOrder()
        {
            return Ok(await _orderService.CreateOrder());
        }
        
        [Authorize]
        [HttpGet("{orderId:long}/detail")]
        public async Task<ActionResult<BaseResponse<OrderDto>>> GetOrderDetail(long orderId)
        {
            return Ok(await _orderService.GetOrderDetail(orderId));
        }
    }
}
