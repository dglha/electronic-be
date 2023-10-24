using Electronic.Application.Contracts.DTOs.Brand;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Persistence.DatabaseContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    /// <summary>
    /// Brand Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service, ElectronicDatabaseContext dbContext)
        {
            _service = service;
        }
        
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateBrandResultDto>> CreateBrand(CreateBrandDto request)
        {
            return Ok( await _service.CreateBrand(request));
        }

        /// <summary>
        /// Gets available brands.
        /// </summary>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="take">Item per page</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pagination<BrandDto>>> GetAllBrands(int pageNumber = 1, int take = 10)
        {
            return Ok(await _service.GetAvailableBrands(pageNumber, take));
        }
        
        /// <summary>
        /// Delete brand.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{brandId:int}/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteBrandWithId(int brandId)
        {
            await _service.DeleteBrand(brandId);
        }
        
        
        /// <summary>
        /// Toggle publish brand.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{brandId:int}/publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task PublishBrandWithId(int brandId)
        {
            await _service.TogglePublishBrand(brandId);
        }
    }
}
