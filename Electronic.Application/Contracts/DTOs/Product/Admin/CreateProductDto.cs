using Electronic.Application.Contracts.Common;
using Electronic.Application.Contracts.DTOs.Category;

namespace Electronic.Application.Contracts.DTOs.Product;

public class CreateProductDto
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string SKU { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public string Specification { get; set; }
    public decimal Price { get; set; }
    public decimal OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsAllowToOrder { get; set; }
    public bool IsVisibleIndividually { get; set; }
    public int StockQuantity { get; set; }
    public int BrandId { get; set; }
    public ImageFileDto ThumbnailImage { get; set; }
    public IEnumerable<ImageFileDto> ProductImages { get; set; }
    public IEnumerable<long> CategoryIds { get; set; } = new List<long>();
}