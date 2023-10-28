using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.ProductOption;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        /// <summary>
        /// Create category.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateProductDto>> CreateProduct([FromForm] CreateProductDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            return Ok( await _productService.CreateProduct(request));
        }
        
        /// <summary>
        /// Add option to product.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{productId:long}/options")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddProductOption(long productId, [FromForm] AddProductOptionValueDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _productService.AddOptionToProduct(productId, request.OptionId, request.Values!);
            return Ok( );
        }
        
        // [Authorize(Roles = "Administrator")]
        // [HttpPost]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<ActionResult<CreateProductDto>> AddProductVariant([FromForm] CreateProductDto request)
        // {
        //     if (!ModelState.IsValid) return BadRequest(ModelState);
        //     
        //     return Ok( await _productService.CreateProduct(request));
        // }
    }
}
