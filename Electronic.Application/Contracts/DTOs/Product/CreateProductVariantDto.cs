using Electronic.Application.Contracts.Common;
using Electronic.Application.Contracts.DTOs.ProductOptionGroup;

namespace Electronic.Application.Contracts.DTOs.Product;

public class CreateProductVariantDto
{
    public string Name { get; set; }
    public string Slug { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public ImageFileDto ThumbnailImage { get; set; }
    public IEnumerable<ImageFileDto?> NewImages { get; set; }
    public IEnumerable<CreateProductOptionGroupDto> OptionCombinations { get; set; }
}