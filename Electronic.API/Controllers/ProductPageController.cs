using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Product.User;
using Electronic.Application.Contracts.DTOs.Review;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
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
        
        
        /// <summary>
        /// Get product detail via slug
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<ActionResult<BaseResponse<ProductDetailUserDto>>> GetProductDetailSlug([FromQuery] string slug)
        {
            return Ok(await _productService.GetProductUserDetailSlug(slug));
        }
        
        /// <summary>
        /// Get Shop's top sell
        /// </summary>
        /// <returns></returns>
        [HttpGet("top-sold")]
        public async Task<ActionResult<BaseResponse<ProductUserDto>>> GetTopSold()
        {
            return Ok(await _productService.GetTopSaleProducts());
        }
        
        /// <summary>
        /// Get Product's reviews
        /// </summary>
        /// <returns></returns>
        [HttpGet("{productId:long}/reviews")]
        public async Task<ActionResult<Pagination<ProductReviewDto>>> GetProductReviews(long productId, int pageNumber = 1, int pageSize = 15)
        {
            return Ok(await _productService.GetProductReview(productId, pageNumber, pageSize));
        }
        
        /// <summary>
        /// Add Product review
        /// </summary>
        /// <returns></returns>
        [HttpPost("{productId:long}/reviews")]
        [Authorize]
        public async Task<ActionResult> AddProductReview(long productId, ProductReviewRequestDto request)
        {
            await _productService.AddProductReview(productId, User.FindFirstValue(ClaimTypes.Email), request);
            return Ok();
        }
    }
}
