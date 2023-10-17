using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Identity;

public class RegistrationRequestDto
{
    [Required] [EmailAddress] public string Email { get; set; }
    [Required] [MinLength(6)] public string Username { get; set; }
    [Required] [MinLength(8)] public string Password { get; set; }
}