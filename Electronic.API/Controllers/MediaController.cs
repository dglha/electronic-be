using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadMedia(IFormFile image)
        {
            await _mediaService.SaveMediaAsync(image.OpenReadStream(), image.FileName);
            return Ok();
        }
        
        [HttpGet]
        public string GetMedia(string mediaName)
        {
            return _mediaService.GetMediaUrl(mediaName);
        }
    }
}
