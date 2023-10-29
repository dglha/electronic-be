using System.ComponentModel.DataAnnotations;

namespace Electronic.API.Requests;

public class CreateProductRequestForm
{
    [Required]
    public string Name { get; set; }
    public string Slug { get; set; }
    [Required]
    public string SKU { get; set; }
    [Required]
    public string ShortDescription { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Specification { get; set; }
    [Required]
    public decimal Price { get; set; }
    public decimal OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
    [Required]
    public bool? IsPublished { get; set; }
    [Required]
    public bool? IsFeatured { get; set; }
    [Required]
    public bool? IsAllowToOrder { get; set; }
    [Required]
    public int? BrandId { get; set; }
    [Required]
    public IEnumerable<long> CategoryIds { get; set; } = new List<long>();
    [Required]
    public IFormFile? ThumbnailImage { get; set; }
    public IList<IFormFile> ProductImages { get; set; }
}