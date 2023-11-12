using Electronic.Application.Contracts.DTOs.Brand;
using Electronic.Application.Contracts.Response;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Application.Interfaces.Services;

public interface IBrandService
{
    Task<CreateBrandResultDto> CreateBrand(CreateBrandDto request);
    Task DeleteBrand(int brandId);

    Task UpdateBrand(int brandId, UpdateBrandDto request);
    Task TogglePublishBrand(int brandId);
    Task<Pagination<BrandDto>> GetAvailableBrands(int pageNumber, int itemPerPage);
}