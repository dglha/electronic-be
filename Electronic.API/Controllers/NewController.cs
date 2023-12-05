using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.API.Requests;
using Electronic.Application.Contracts.Common;
using Electronic.Application.Contracts.DTOs.New;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Persistences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewController : ControllerBase
    {
        private readonly INewService _newService;

        public NewController(INewService newService)
        {
            _newService = newService;
        }

        /// <summary>
        /// Add new category for New
        /// </summary>
        /// <returns></returns>
        [HttpPost("add-new-category")]
        public async Task<ActionResult<NewCategoryDto>> CreateNewCategory(CreateNewCategoryDto request)
            => Ok(await _newService.AddNewCategory(request));

        /// <summary>
        /// Add NewItem
        /// </summary>
        /// <returns></returns>
        [HttpPost("add-new-item")]
        public async Task<ActionResult> CreateNewItem([FromForm] CreateNewRequestForm request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newRequest = new CreateNewItemDto
            {
                IsPublished = request.IsPublished,
                IsDeleted = request.IsDeleted,
                ThumbnailImage = request.ThumbnailImage != null
                    ? new ImageFileDto
                    {
                        FileName = request.ThumbnailImage.FileName,
                        FileType = request.ThumbnailImage.ContentType,
                        FileContent = request.ThumbnailImage.OpenReadStream()
                    }
                    : null,
                Title = request.Title,
                FullContent = request.FullContent,
                NewCatetoryIds = request.NewCatetoryIds,
                ShortContent = request.ShortContent
            };

            await _newService.AddNewItem(newRequest);
            return Ok();
        }
        
        /// <summary>
        /// Get all new category
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("newCategories")]
        public async Task<ActionResult<BaseResponse<ICollection<NewItemDto>>>> GetAllNewCategories()
        {
            return Ok(await _newService.GetNewCategories());
        }

        /// <summary>
        /// Get all new item (Admin)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("newItems")]
        public async Task<ActionResult<Pagination<NewItemDto>>> GetAllNewItems(int pageIndex = 1, int itemPerPage = 15)
        {
            return Ok(await _newService.GetNewItems(pageIndex, itemPerPage));
        }
    }
}
