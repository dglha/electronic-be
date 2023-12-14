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
        [HttpPost("{orderId:long}/Address")]
        public async Task<ActionResult> UpdateOrderAddress(long orderId, OrderAddressDto request)
        {
            await _orderService.UpdateOrderAddress(request);
            return Ok();
        }
        
        
        [Authorize]
        [HttpGet("{orderId:long}/detail")]
        public async Task<ActionResult<BaseResponse<OrderDto>>> GetOrderDetail(long orderId)
        {
            return Ok(await _orderService.GetOrderDetail(orderId));
        }
        
        /// <summary>
        /// Get admin order list
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("list")]
        public async Task<ActionResult<Pagination<OrderListDto>>> GetAdminOrders(int pageNumber = 1, int take = 20)
        {
            return Ok(await _orderService.GetOrders(pageNumber, take));
        }
        
        /// <summary>
        /// Get user order list
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("user-orders")]
        public async Task<ActionResult<Pagination<OrderListDto>>> GetUserOrders(int pageNumber = 1, int take = 20)
        {
            return Ok(await _orderService.GetOrdersByUser(pageNumber, take));
        }
    }
}
