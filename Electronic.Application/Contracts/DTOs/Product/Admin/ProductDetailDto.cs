namespace Electronic.Application.Contracts.DTOs.Product;

public class ProductDetailDto
{
    public long ProductId { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public string Specification { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public string Slug { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
    public bool HasOption { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsAllowToOrder { get; set; }
    public bool IsVisibleIndividually { get; set; }
    public bool IsNewProduct { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }
    public int? StockQuantity { get; set; }
    public int? BrandId { get; set; }
    public string Brand { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public IEnumerable<string> MediasUrl { get; set; }
    public IEnumerable<ProductOptionValueDto> OptionValues { get; set; }
    public IEnumerable<ProductCategoryDto> Categories { get; set; }
    public IEnumerable<ProductLinkDto> ProductLinks { get; set; }
    public IEnumerable<ProductOptionCombinationDto> OptionCombinations { get; set; }
}

public class ProductOptionCombinationDto
{
    public string OptionName { get; set; }
    public string Value { get; set; }
}

public class ProductOptionValueDto
{
    public string ProductOption { get; set; }
    public List<string>? Value { get; set; }
}

public class ProductCategoryDto
{
    public long CategoryId { get; set; }
    public string CategoryName { get; set; }
}

public class ProductLinkDto
{
    public int LinkType { get; set; }
    public long ProductId { get; set; }
}