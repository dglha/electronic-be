using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.ProductOption;

namespace Electronic.Application.Interfaces.Services;

public interface IProductOptionService
{
    Task<ProductOptionDto> CreateProductOption(CreateProductOptionDto request);
    Task<IEnumerable<ProductOptionDto>> GetListProductOption();
    Task<ProductOptionDto> UpdateProductOption(int productOptionId, CreateProductOptionDto request);
    
    Task DeleteProductOption(int productOptionId);
}