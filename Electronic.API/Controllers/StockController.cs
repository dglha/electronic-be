using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Stock.Admin;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        /// <summary>
        /// Update product quantity (stock)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{productId:long}/update-quantity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateProductQuantity(UpdateProductStockRequestDto request)
        {
            await _stockService.UpdateProductStockQuantity(request);
            return Ok();
        }
        
        /// <summary>
        /// Get List product Stock
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pagination<ProductStockDto>>> GetProductStockList(string? productName, int pageNumber = 1, int itemPerPage = 15)
        {
            
            return Ok(await _stockService.GetListStock(productName, pageNumber, itemPerPage));
        }
        
        /// <summary>
        /// Get List product Stock history
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("{productId:long}/stock-history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pagination<ProductStockHistoryDto>>> GetProductStockList(long productId, int pageNumber = 1, int itemPerPage = 15)
        {
            return Ok(await _stockService.GetProductStockHistory(productId, pageNumber, itemPerPage));
        }
        
    }
}
