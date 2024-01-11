using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Address;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AddressesController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        /// <summary>
        /// Get Ads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<BaseResponse<List<AddressDto>>>> GetUserAddresses()
        {
            return Ok(await _orderService.GetUserAddresses());
        }
    }
}
