using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Identity;

public class CheckEmailRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}