using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.ShoppingCart;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<CartDto>>> UpdateCart(UpdateCartDto request)
        {
            return Ok(await _shoppingCartService.UpdateCart(request));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CartDto>>> GetCart()
        {
            return Ok(await _shoppingCartService.GetCart());
        }
        
        [Authorize]
        [HttpPost("add-cart")]
        public async Task<ActionResult> AddToCart(long productId)
        {
            await _shoppingCartService.AddToCart(productId);
            return Ok();
        }
    }
}