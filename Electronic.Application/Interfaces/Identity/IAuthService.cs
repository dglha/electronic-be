using Electronic.Application.Contracts.DTOs.Identity;

namespace Electronic.Application.Contracts.Identity;

public interface IAuthService
{
    Task<AuthResonseDto> Login(AuthRequestDto request);
    Task<AuthResonseDto> Register(RegistrationRequestDto request);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<ApplicationUserDto> GetCurrentUserInfo(string email);
}