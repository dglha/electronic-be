using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.ProductOption;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOptionController : ControllerBase
    {
        private readonly IProductOptionService _productOptionService;

        public ProductOptionController(IProductOptionService productOptionService)
        {
            _productOptionService = productOptionService;
        }
        
        /// <summary>
        /// Create Product Option.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("new")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductOptionDto>> CreateProductOption([FromBody] CreateProductOptionDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            return Ok( await _productOptionService.CreateProductOption(request));
        }
        
        /// <summary>
        /// Get list of Product Option.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("get-list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductOptionDto>> GetProductOptions()
        {
            return Ok( await _productOptionService.GetListProductOption());
        }
        
        /// <summary>
        /// Update Product Option.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id:int}/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductOptionDto>> UpdateProductOption(int id, CreateProductOptionDto request)
        {
            return Ok( await _productOptionService.UpdateProductOption(id, request));
        }
        
        /// <summary>
        /// Delete Product Option.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteProductOptions(int id)
        {
            await _productOptionService.DeleteProductOption(id);
            return Ok();
        }
    }
}
