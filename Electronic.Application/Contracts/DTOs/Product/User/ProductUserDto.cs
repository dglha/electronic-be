namespace Electronic.Application.Contracts.DTOs.Product.User;

public class ProductUserDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public decimal? SpecialPrice { get; set; }
    
    public string ThumbnailImage { get; set; }
}