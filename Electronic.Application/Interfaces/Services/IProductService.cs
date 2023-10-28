using Electronic.Application.Contracts.DTOs.Product;

namespace Electronic.Application.Interfaces.Services;

public interface IProductService
{
    Task<CreateProductDto> CreateProduct(CreateProductDto request);

    Task AddOptionToProduct(long productId, int optionId, List<string> value);
}