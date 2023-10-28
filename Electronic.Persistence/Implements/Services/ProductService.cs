using System.Net;
using System.Text.Json;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Catalog;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class ProductService : IProductService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IProductRepository _productRepository;
    private readonly IProductOptionRepository _productOptionRepository;
    private readonly IMediaService _mediaService;

    public ProductService(ElectronicDatabaseContext dbContext, IProductRepository productRepository,
        IMediaService mediaService, IProductOptionRepository productOptionRepository)
    {
        _dbContext = dbContext;
        _productRepository = productRepository;
        _mediaService = mediaService;
        _productOptionRepository = productOptionRepository;
    }

    public async Task<CreateProductDto> CreateProduct(CreateProductDto request)
    {
        var product = new Product
        {
            Name = request.Name,
            NormalizedName = request.Name,
            Slug = request.Slug,
            SKU = request.SKU,
            ShortDescription = request.ShortDescription,
            Description = request.Description,
            Specification = request.Specification,
            Price = request.Price,
            OldPrice = request.OldPrice,
            SpecialPrice = request.SpecialPrice,
            SpecialPriceStartDate = request.SpecialPriceStartDate,
            SpecialPriceEndDate = request.SpecialPriceEndDate,
            IsPublished = request.IsPublished,
            IsFeatured = request.IsFeatured,
            IsAllowToOrder = request.IsAllowToOrder,
            BrandId = request.BrandId,
            // HasOptions = request.Variations.Any() ? true : false,
            IsVisibleIndividually = true,
            IsDeleted = false,
            IsNewProduct = true,
            StockQuantity = 100
        };

        foreach (var categoryId in request.CategoryIds)
        {
            var productCategory = new ProductCategory
            {
                CategoryId = categoryId
            };
            product.AddCategory(productCategory);
        }

        // await _productRepository.CreateAsync(product);
        return request;
    }

    public async Task AddOptionToProduct(long productId, int optionId, List<string> values)
    {
        var option = await _productOptionRepository.GetAsync(optionId);
        if (option == null) throw new AppException("Option doesn't exists", (int)HttpStatusCode.BadRequest);
        var product = await _dbContext.Set<Product>().FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null) throw new AppException("Product doesn't exists", (int)HttpStatusCode.BadRequest);
        var productOptionValue = new ProductOptionValue
        {
            ProductOptionId = option.ProductOptionId, ProductId = product.ProductId,
            Value = JsonSerializer.Serialize(values)
        };
        await _dbContext.AddAsync(productOptionValue);
        await _dbContext.SaveChangesAsync();
    }
    
}