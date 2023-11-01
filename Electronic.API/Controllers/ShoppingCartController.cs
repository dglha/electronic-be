using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.ShoppingCart;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCart(UpdateCartDto request)
        {
            await _shoppingCartService.UpdateCart(request);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponse<CartDto>>> GetCart()
        {
            return Ok(await _shoppingCartService.GetCart());
        }
    }
}
