using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.Application.Contracts.DTOs.Product.User;
using Electronic.Application.Contracts.Queries;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchPageQueryController : ControllerBase
    {

        private readonly IProductService _productService;

        public SearchPageQueryController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<ProductUserDto>>> ProductFilter([FromQuery] ProductQuery query)
        {
            return Ok(await _productService.GetProductList(query));
        }
    }
}
