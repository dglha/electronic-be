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
        
        
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateCategoryDto>> CreateCategory(CreateCategoryDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok( await _service.CreateNewCategory(request));
        }
        
    }
}
