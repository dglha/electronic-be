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
    public class ProductPageController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductPageController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{productId:long}/detail")]
        public async Task<ActionResult<BaseResponse<ProductDetailUserDto>>> GetProductDetail(long productId)
        {
            return Ok(await _productService.GetProductUserDetail(productId));
        }
    }
}
