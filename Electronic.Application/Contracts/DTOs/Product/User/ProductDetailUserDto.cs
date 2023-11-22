namespace Electronic.Application.Contracts.DTOs.Product.User;

public class ProductDetailUserDto
{
    public long ProductId { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
    public int? StockQuantity { get; set; }
    public int? BrandId { get; set; }
    public string Brand { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public IEnumerable<string> MediasUrl { get; set; }
    public IEnumerable<ProductOptionValueDto> OptionValues { get; set; }
    public IEnumerable<ProductCategoryDto> Categories { get; set; }
    // public IEnumerable<ProductLinkDto> ProductLinks { get; set; }
    public IEnumerable<ProductOptionCombinationDto> OptionCombinations { get; set; }
    public IEnumerable<ProductDetailUserDto> ProductVariants { get; set; }
}