using System.Net;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.ProductOption;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Catalog;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class ProductOptionService : IProductOptionService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IProductOptionRepository _productOptionRepository;

    public ProductOptionService(ElectronicDatabaseContext dbContext, IProductOptionRepository productOptionRepository)
    {
        _dbContext = dbContext;
        _productOptionRepository = productOptionRepository;
    }

    public async Task<ProductOptionDto> CreateProductOption(CreateProductOptionDto request)
    {
        var productOption = new ProductOption
        {
            Name = request.Name
        };

        await _productOptionRepository.CreateAsync(productOption);

        return new ProductOptionDto
        {
            ProductOptionId = productOption.ProductOptionId,
            Name = productOption.Name
        };
    }

    public async Task<IEnumerable<ProductOptionDto>> GetListProductOption()
    {
        return await _dbContext.Set<ProductOption>().Select(po => new ProductOptionDto {Name = po.Name, ProductOptionId = po.ProductOptionId}).ToListAsync();
    }

    public async Task<ProductOptionDto> UpdateProductOption(int productOptionId, CreateProductOptionDto request)
    {
        var productOption = await _productOptionRepository.GetAsync(productOptionId);
        if (productOption == null)
            throw new AppException("Product option not found!", (int)HttpStatusCode.BadRequest);
        productOption.Name = request.Name;
        await _dbContext.SaveChangesAsync();
        return new ProductOptionDto
        {
            ProductOptionId = productOption.ProductOptionId,
            Name = productOption.Name
        };
    }

    public async Task DeleteProductOption(int productOptionId)
    {
        var productOption = await _productOptionRepository.GetAsync(productOptionId);
        if (productOption == null)
            throw new AppException("Product option not found!", (int)HttpStatusCode.BadRequest);
        await _productOptionRepository.DeleteAsync(productOption);
    }
}