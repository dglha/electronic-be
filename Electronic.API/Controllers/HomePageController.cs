using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Product.User;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {

        private readonly IProductService _productService;

        public HomePageController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get featured products
        /// </summary>
        /// <returns></returns>
        [HttpGet("feature-product")]
        public async Task<ActionResult<BaseResponse<ProductUserDto>>> GetFeaturedProducts()
        {
            return Ok(await _productService.GetFeaturedProducts());
        }
        
        /// <summary>
        /// Get new products
        /// </summary>
        /// <returns></returns>
        [HttpGet("new-product")]
        public async Task<ActionResult<BaseResponse<ProductUserDto>>> GetNewProducts()
        {
            return Ok(await _productService.GetNewProducts());
        }
        
        /// <summary>
        /// Get category product (only take random 5)
        /// </summary>
        /// <returns></returns>
        [HttpGet("list-product")]
        public async Task<ActionResult<BaseResponse<ProductUserDto>>> GetProductCategory([FromQuery] long categoryId)
        {
            return Ok(await _productService.GetTopProductCategory(categoryId));
        }
    }
}
