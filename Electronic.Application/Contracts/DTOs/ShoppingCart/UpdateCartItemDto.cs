using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.ShoppingCart;

public class UpdateCartItemDto
{
    [Required]
    public long ProductId { get; set; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
}