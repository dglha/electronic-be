using Electronic.Application.Contracts.DTOs.New;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Persistences;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewPageController : ControllerBase
    {
        private readonly INewService _newService;

        public NewPageController(INewService newService)
        {
            _newService = newService;
        }
        
        /// <summary>
        /// Get new categories
        /// </summary>
        /// <returns></returns>
        [HttpGet("categories")]
        public async Task<ActionResult<BaseResponse<ICollection<NewCategoryDto>>>> GetNewCategories()
            => Ok(await _newService.GetUserNewCategories());
        
        /// <summary>
        /// Get list New Items
        /// </summary>
        /// <returns></returns>
        [HttpGet("items")]
        public async Task<ActionResult<Pagination<NewItemDto>>> GetNewItems(int categoryId = 0, int pageNumber = 1, int take = 15)
            => Ok(await _newService.GetUserNewItems(pageNumber, take, categoryId));
        
        /// <summary>
        /// Get new content (detail)
        /// </summary>
        /// <returns></returns>
        [HttpGet("item/{newItemId:int}/detail")]
        public async Task<ActionResult<BaseResponse<NewItemDetailDto>>> GetNewItemDetail(int newItemId)
            => Ok(await _newService.GetNewItemDetails(newItemId));
        
        /// <summary>
        /// Get new content (detail) - via Slug
        /// </summary>
        /// <returns></returns>
        [HttpGet("item/detail")]
        public async Task<ActionResult<BaseResponse<NewItemDetailDto>>> GetNewItemDetailSlug([FromQuery] string slug)
            => Ok(await _newService.GetNewItemDetailSlug(slug));
    }
}
