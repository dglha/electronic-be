using Electronic.API.Requests;
using Electronic.Application.Contracts.DTOs.Category;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }
        
        /// <summary>
        /// Get categories.
        /// </summary>
        /// <param name="pageNumber">Current page number.</param>
        /// <param name="take">Item per page</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pagination<CategoryDto>>> GetAllCategories(int pageNumber = 1, int take = 10)
        {
            return Ok(await _service.GetAllCategories(pageNumber, take));
        }
        
        /// <summary>
        /// Delete category.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{categoryId:long}/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteCategoryWithId(int categoryId)
        {
            await _service.DeleteCategory(categoryId);
        }
        
        /// <summary>
        /// Create category.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateCategoryDto>> CreateCategory([FromForm] CategoryRequestForm request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            return Ok( await _service.CreateNewCategory(request, request.ThumbnailImage.OpenReadStream(), request.ThumbnailImage.FileName));
        }
        
        /// <summary>
        /// Update category child
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{categoryId:long}/child")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateCategoryChild(long categoryId, List<long> childIds)
        {
            await _service.UpdateCategoryChildren(categoryId, childIds);
            return Ok();
        }
        
        /// <summary>
        /// Get available category list for updating parent category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("available-category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<CategoryDto>>> GetListAvailableCategory()
        {
            return Ok( await _service.GetListAvailableCategory());
        }
        
        /// <summary>
        /// Update category info
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{categoryId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<CategoryDto>>> UpdateCategoryChild(long categoryId, UpdateCategoryRequestDto request)
        {
            return Ok( await _service.UpdateCategory(categoryId, request));
        }
        
        
    }
}
