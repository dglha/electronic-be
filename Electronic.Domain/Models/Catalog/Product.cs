using Electronic.Domain.Common;
using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;

namespace Electronic.Domain.Model.Catalog;

public class Product : BaseEntity
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
    public decimal OldPrice { get; set; }
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
    public int? RatingCount { get; set; }
    public double? RatingAverage { get; set; }
    public int StockQuantity { get; set; }
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public long? ThumbnailImageId { get; set; }
    public Media? ThumbnailImage { get; set; }
    public ICollection<ProductMedia> Medias { get; set; }
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<ProductOptionValue> OptionValues { get; set; } = new List<ProductOptionValue>();
    public ICollection<ProductOptionGroup> OptionCombinations { get; set; } = new List<ProductOptionGroup>();
    public ICollection<ProductCategory> Categories { get; set; }
    public ICollection<ProductPriceHistory> PriceHistories { get; set; }
    public ICollection<ProductLink> ProductLinks { get; set; } = new List<ProductLink>();
}