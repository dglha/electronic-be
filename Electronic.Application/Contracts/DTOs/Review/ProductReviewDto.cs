namespace Electronic.Application.Contracts.DTOs.Review;

public class ProductReviewDto
{
    public string ReviewerName { get; set; }
    public string ReviewerId { get; set; }
    public long ReviewId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
}