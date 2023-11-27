using System.ComponentModel.DataAnnotations;

namespace Electronic.Application.Contracts.DTOs.Review;

public class ProductReviewRequestDto
{
    public string Comment { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
}