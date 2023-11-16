using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IProductService
{
    Task<CreateProductDto> CreateProduct(CreateProductDto request);

    Task AddOptionToProduct(long productId, int optionId, List<string> value);

    Task AddProductVariant(long parentProductId, CreateProductVariantDto request);

    Task UpdateProduct(long productId, UpdateProductDto request);

    Task UpdateProductVariant(long parentProductId, IEnumerable<UpdateProductVariantDto> productVariantList);

    Task UpdateProductOption(long productId, IList<UpdateProductOptionDto> updateProductOptionDtos);

    Task<Pagination<ProductListDto>> GetProductList(int pageIndex, int itemPerPage);

    Task<BaseResponse<ProductDetailDto>> GetProductDetail(long productId);

    Task<BaseResponse<string>> UpdateProductPrice(long productId, UpdateProductPriceRequestDto request);

    Task<IEnumerable<ProductOptionValueDto>> GetProductOptionsDetail(long productId);
}