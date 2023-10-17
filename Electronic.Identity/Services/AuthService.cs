using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Electronic.Application.Contracts.DTOs.Identity;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Identity.Extensions;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAppLogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSetting, IAppLogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSetting.Value;
        _logger = logger;
    }

    public async Task<AuthResonseDto> Login(AuthRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) throw new Exception("Not found");
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result) throw new Exception($"Credential for {request.Email} aren't valid.");

        var jwtSecurityToken = await GenerateToken(user);
        var response = new AuthResonseDto
        {
            Id = user.Id,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email,
            Username = user.UserName,
        };
        
        _logger.LogInformation($"{response.Username} Logged!!!");

        return response;
    }

    public async Task<AuthResonseDto> Register(RegistrationRequestDto request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Username,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception("Error when register new account");
        }

        await _userManager.AddToRoleAsync(user, "Customer");

        var jwtSecurityToken = await GenerateToken(user);
        var response = new AuthResonseDto
        {
            Id = user.Id,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email,
            Username = user.UserName,
        };

        return response;
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<ApplicationUserDto> GetCurrentUserInfo(string email)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
        return new ApplicationUserDto
        {
            Username = user.UserName, Email = user.Email, Id = user.Id
        };
    }

    private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

        var claim = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }.Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claim,
            expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
            signingCredentials: signingCredentials);
        return jwtSecurityToken;
    }
}