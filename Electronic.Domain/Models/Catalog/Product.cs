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
    public int? RatingCount { get; set; }
    public double? RatingAverage { get; set; }
    public int? StockQuantity { get; set; }
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public long? ThumbnailImageId { get; set; }
    
    public Media? ThumbnailImage { get; set; }
    public ICollection<ProductMedia> Medias { get; set; } = new List<ProductMedia>();
    public ICollection<ProductAttributeValue> AttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<ProductOptionValue> OptionValues { get; set; } = new List<ProductOptionValue>();
    public ICollection<ProductOptionGroup> OptionCombinations { get; set; } = new List<ProductOptionGroup>();
    public ICollection<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
    public ICollection<ProductPriceHistory> PriceHistories { get; set; } = new List<ProductPriceHistory>();
    public ICollection<ProductLink> ProductLinks { get; set; } = new List<ProductLink>();
    
    public void AddCategory(ProductCategory category)
    {
        category.Product = this;
        Categories.Add(category);
    }
    
    public void AddMedia(ProductMedia media)
    {
        media.Product = this;
        Medias.Add(media);
    }
    
    public void AddOptionCombination(ProductOptionGroup combination)
    {
        combination.Product = this;
        OptionCombinations.Add(combination);
    }
    
    public void AddProductLinks(ProductLink productLink)
    {
        productLink.Product = this;
        ProductLinks.Add(productLink);
    }
    
    public Product Clone()
        {
            var product = new Product();
            product.Name = Name;
            product.ShortDescription = ShortDescription;
            product.Description = Description;
            product.Specification = Specification;
            product.IsPublished = IsPublished;
            product.Price = Price;
            product.OldPrice = OldPrice;
            product.SpecialPrice = SpecialPrice;
            product.SpecialPriceStartDate = SpecialPriceStartDate;
            product.SpecialPriceEndDate = SpecialPriceEndDate;
            product.HasOption = HasOption;
            product.IsVisibleIndividually = IsVisibleIndividually;
            product.IsFeatured = IsFeatured;
            product.IsAllowToOrder = IsAllowToOrder;
            product.StockQuantity = StockQuantity;
            product.BrandId = BrandId;
            product.SKU = SKU;
            product.NormalizedName = NormalizedName;
            product.Slug = Slug;
            
            foreach (var category in Categories)
            {
                product.AddCategory(new ProductCategory
                {
                    CategoryId = category.CategoryId
                });
            }

            return product;
        }
}