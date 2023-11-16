namespace Electronic.Application.Contracts.DTOs.Product;

public class ProductListDto
{
    public long ProductId { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public decimal Price { get; set; }
    public decimal SpecialPrice { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsAllowToOrder { get; set; }
    public bool IsVisibleIndividually { get; set; }
    public bool HasOption { get; set; }
    public int? StockQuantity { get; set; }
    public string Brand { get; set; }
}