using System.Security.Claims;
using Electronic.Application.Contracts.DTOs.Identity;
using Electronic.Application.Contracts.Identity;
using Identity.Extensions;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthService _authService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<ApplicationUserDto>> GetCurrentUser()
            => await _authService.GetCurrentUserInfo(User.FindFirstValue(ClaimTypes.Email));


        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] CheckEmailRequestDto query)
            => await _authService.CheckEmailExistsAsync(query.Email);

        [HttpPost("login")]
        public async Task<ActionResult<AuthResonseDto>> Login(AuthRequestDto request)
            => await _authService.Login(request);

        [HttpPost("register")]
        public async Task<ActionResult<AuthResonseDto>> Register(RegistrationRequestDto request) =>
            await _authService.Register(request);
    }
}