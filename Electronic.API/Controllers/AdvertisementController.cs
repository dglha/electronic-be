using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electronic.API.Requests;
using Electronic.Application.Contracts.Common;
using Electronic.Application.Contracts.DTOs.Advertisement;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        /// <summary>
        /// Add new AD
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> CreateNewAd([FromForm] CreateAdForm request)
        {
            var newRequest = new CreateAdvertisementDto
            {
                Name = request.Name,
                DisplayOrder = request.DisplayOrder,
                Description = request.Description,
                Image = new ImageFileDto
                {
                    FileName = request.Image.FileName,
                    FileType = request.Image.ContentType,
                    FileContent = request.Image.OpenReadStream()
                },
            };
            await _advertisementService.AddNewAdvertisement(newRequest);
            return Ok();
        }
        
        /// <summary>
        /// Get Ads
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<AdvertisementDto>>> GetAds()
        {
            return Ok(await _advertisementService.GetAd());
        }
    }
}
