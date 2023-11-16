using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.ProductOptionGroup;

namespace Electronic.API.Requests;

public class CreateProductVariantRequestForm
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public IFormFile ThumbnailImage { get; set; }
    public IEnumerable<IFormFile?> NewImages { get; set; }
    public IEnumerable<CreateProductOptionGroupDto> OptionCombinations { get; set; }
}