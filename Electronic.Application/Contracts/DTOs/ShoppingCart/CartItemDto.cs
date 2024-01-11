namespace Electronic.Application.Contracts.DTOs.ShoppingCart;

public class CartItemDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public int Quantity { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public decimal TotalPrice { get; set; }
    public List<string> Options { get; set; }
    public string Slug { get; set; }
    public long ProductId { get; set; }
    public bool IsAvailable { get; set; }
    public string? Note { get; set; }
}