﻿using System.Net;
using System.Text.Json;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class ProductService : IProductService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IProductRepository _productRepository;
    private readonly IProductOptionRepository _productOptionRepository;
    private readonly IMediaService _mediaService;
    private readonly IAppLogger<ProductService> _logger;

    public ProductService(ElectronicDatabaseContext dbContext, IProductRepository productRepository,
        IMediaService mediaService, IProductOptionRepository productOptionRepository, IAppLogger<ProductService> logger)
    {
        _dbContext = dbContext;
        _productRepository = productRepository;
        _mediaService = mediaService;
        _productOptionRepository = productOptionRepository;
        _logger = logger;
    }

    public async Task<CreateProductDto> CreateProduct(CreateProductDto request)
    {
        if (!request.CategoryIds.Any())
            throw new AppException("Invalid Category id(s)", (int)HttpStatusCode.BadRequest);
        var product = new Product
        {
            Name = request.Name,
            NormalizedName = request.Name,
            Slug = request.Name.ToUrlFriendly(),
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
            HasOption = false,
            IsVisibleIndividually = request.IsVisibleIndividually,
            IsDeleted = false,
            IsNewProduct = true,
            StockQuantity = request.IsVisibleIndividually ? request.StockQuantity : null
        };

        foreach (var categoryId in request.CategoryIds)
        {
            var productCategory = new ProductCategory
            {
                CategoryId = categoryId
            };
            product.AddCategory(productCategory);
        }

        var productPriceHistory = CreatePriceHistory(product);
        product.PriceHistories.Add(productPriceHistory);

        await _productRepository.CreateAsync(product);
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

    public async Task AddProductVariant(long parentProductId, CreateProductVariantDto request)
    {
        var product = _dbContext.Set<Product>().Include(product => product.ThumbnailImage)
            .FirstOrDefault(p => p.ProductId == parentProductId);

        if (product == null) throw new AppException("Product doesn't exists!!!", (int)HttpStatusCode.BadRequest);

        var productOptionIds = request.OptionCombinations.Select(o => o.ProductOptionId).ToList();

        var isAllOptionExist =
            productOptionIds.All(id => _dbContext.Set<ProductOption>().Any(x => x.ProductOptionId == id));

        if (!isAllOptionExist)
            throw new AppException("One or more Product Option doesn't exists!!!", (int)HttpStatusCode.BadRequest);

        // Create Variant Product
        var variantProduct = product.Clone();

        variantProduct.Name = request.Name;
        variantProduct.Slug = request.Name.ToUrlFriendly();
        variantProduct.SKU = request.SKU;
        variantProduct.Price = request.Price;
        variantProduct.OldPrice = request.OldPrice;
        variantProduct.HasOption = false;
        variantProduct.IsVisibleIndividually = false;

        // Upload image
        if (product.ThumbnailImage != null)
        {
            variantProduct.ThumbnailImage = new Media
                { FileName = product.ThumbnailImage.FileName, Caption = "", MediaType = MediaTypeEnum.Image };
        }

        await MapProductVariantImageFromRequest(request, variantProduct);

        // Add option group (Combination)
        foreach (var optionCombination in request.OptionCombinations)
        {
            variantProduct.AddOptionCombination(new ProductOptionGroup
            {
                ProductOptionId = (int)optionCombination.ProductOptionId!,
                Value = optionCombination.Value,
            });
        }

        // Create price history
        var productPriceHistory = CreatePriceHistory(variantProduct);
        variantProduct.PriceHistories.Add(productPriceHistory);

        // Create product link
        var productLink = new ProductLink
        {
            Type = ProductLinkEnum.Variant,
            Product = product,
            LinkedProduct = variantProduct
        };

        product.AddProductLinks(productLink);
        if (!product.HasOption) product.HasOption = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateProduct(long productId, UpdateProductDto request)
    {
        var product = await _dbContext.Set<Product>()
            .Include(p => p.ThumbnailImage)
            .Include(p => p.Medias).ThenInclude(m => m.Media)
            .Include(p => p.Categories)
            .FirstOrDefaultAsync();

        if (product == null) throw new AppException("Product doesn't exists", (int)HttpStatusCode.BadRequest);

        var isPriceChanged = product.Price != request.Price ||
                             product.OldPrice != request.OldPrice ||
                             product.SpecialPrice != request.SpecialPrice ||
                             product.SpecialPriceStartDate != request.SpecialPriceStartDate ||
                             product.SpecialPriceEndDate != request.SpecialPriceEndDate;

        // Update product
        product.Name = request.Name;
        product.Slug = request.Slug;
        product.SKU = request.SKU;
        product.ShortDescription = request.ShortDescription;
        product.Description = request.Description;
        product.SpecialPrice = request.SpecialPrice;
        product.OldPrice = request.OldPrice;
        product.SpecialPriceStartDate = request.SpecialPriceEndDate;
        product.IsPublished = request.IsPublished;
        product.IsFeatured = request.IsFeatured;
        product.IsAllowToOrder = request.IsAllowToOrder;
        product.BrandId = request.BrandId;

        if (isPriceChanged)
        {
            var productPriceHistory = CreatePriceHistory(product);
            product.PriceHistories.Add(productPriceHistory);
        }

        await SaveProductMedias(request, product);

        foreach (var productMediaId in request.DeletedMediaIds)
        {
            var productMedia = product.Medias.First(x => x.MediaId == productMediaId);
            _dbContext.Set<ProductMedia>().Remove(productMedia);
            await _mediaService.DeleteMediaAsync(productMedia.Media);
        }

        AddOrDeleteCategories(request, product);
    }

    public async Task UpdateProductVariant(long parentProductId,
        IEnumerable<UpdateProductVariantDto> productVariantList)
    {
        foreach (var request in productVariantList)
        {
            var product = await _dbContext.Set<Product>().FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
            if (product == null) throw new AppException("Variant doesn't exists", 400);
            var isPriceChanged = product.Price != request.Price ||
                                 product.SpecialPrice != request.SpecialPrice ||
                                 product.SpecialPriceStartDate != request.SpecialPriceStartDate ||
                                 product.SpecialPriceEndDate != request.SpecialPriceEndDate;
            if (!isPriceChanged) continue;
            product.OldPrice = product.Price;
            product.Price = request.Price;
            product.SpecialPrice = request.SpecialPrice;
            product.SpecialPriceStartDate = request.SpecialPriceStartDate;
            product.SpecialPriceEndDate = request.SpecialPriceEndDate;

            var productPriceHistory = CreatePriceHistory(product);
            product.PriceHistories.Add(productPriceHistory);
        }

        var productVariantIds = productVariantList.Select(x => x.ProductId).ToList();
        var deletedProductVariant = await _dbContext.Set<ProductLink>()
            .Include(x => x.LinkedProduct)
            .Where(x =>
                x.ProductId == parentProductId 
                && !productVariantIds.Contains(x.LinkedProductId)
                && x.Type == ProductLinkEnum.Variant)
            .ToListAsync();

        foreach (var deletedProductLink in deletedProductVariant)
        {
            deletedProductLink.LinkedProduct.IsDeleted = true;
            deletedProductLink.LinkedProduct.IsPublished = false;
            _dbContext.Set<ProductLink>().Remove(deletedProductLink);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateProductOption(long productId, IList<UpdateProductOptionDto> updateProductOptionDtos)
    {
        var product = await _dbContext.Set<Product>()
            .Include(p => p.OptionValues)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null) throw new AppException("Product doesn't exists", 400);
        
        AddOrDeleteProductOption(updateProductOptionDtos, product);
        
    }

    private static ProductPriceHistory CreatePriceHistory(Product product)
    {
        return new ProductPriceHistory
        {
            Product = product,
            Price = product.Price,
            OldPrice = product.OldPrice,
            SpecialPrice = product.SpecialPrice,
            SpecialPriceStartDate = product.SpecialPriceStartDate,
            SpecialPriceEndDate = product.SpecialPriceEndDate
        };
    }

    private async Task MapProductVariantImageFromRequest(CreateProductVariantDto request, Product variantProduct)
    {
        if (request.ThumbnailImage != null)
        {
            var thumbnailImageFileName = await SaveFile(request.ThumbnailImage.FileContent,
                request.ThumbnailImage.FileName, request.ThumbnailImage.FileType);
            if (variantProduct.ThumbnailImage != null)
            {
                variantProduct.ThumbnailImage.FileName = thumbnailImageFileName;
            }
            else
            {
                variantProduct.ThumbnailImage = new Media
                    { FileName = thumbnailImageFileName, MediaType = MediaTypeEnum.Image, Caption = "" };
            }
        }

        foreach (var image in request.NewImages)
        {
            if (image == null) continue;
            var fileName = await SaveFile(image.FileContent, image.FileName, image.FileType);
            var productMedia = new ProductMedia
            {
                Product = variantProduct,
                Media = new Media { FileName = fileName, MediaType = MediaTypeEnum.Image, Caption = "" }
            };

            variantProduct.AddMedia(productMedia);
        }
    }

    private async Task<string> SaveFile(Stream mediaBinaryStream, string fileName, string mimeType)
    {
        var originalFileName = fileName.Trim('"');
        var newFileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        await _mediaService.SaveMediaAsync(mediaBinaryStream, newFileName, mimeType);
        return newFileName;
    }

    private async Task SaveProductMedias(CreateProductDto request, Product product)
    {
        if (request.ThumbnailImage != null)
        {
            var fileName = await SaveFile(request.ThumbnailImage.FileContent, request.ThumbnailImage.FileName,
                request.ThumbnailImage.FileType);
            if (product.ThumbnailImage != null)
            {
                product.ThumbnailImage.FileName = fileName;
            }
            else
            {
                product.ThumbnailImage = new Media
                    { FileName = fileName, MediaType = MediaTypeEnum.Image, Caption = "" };
            }
        }

        foreach (var image in request.ProductImages)
        {
            var fileName = await SaveFile(image.FileContent, image.FileName, image.FileType);
            var productMedia = new ProductMedia
            {
                Product = product,
                Media = new Media { FileName = fileName, MediaType = MediaTypeEnum.Image, Caption = "" }
            };
            product.AddMedia(productMedia);
        }
    }

    private void AddOrDeleteCategories(UpdateProductDto request, Product product)
    {
        foreach (var categoryId in request.CategoryIds)
        {
            if (product.Categories.Any(x => x.CategoryId == categoryId)) continue;

            var productCategory = new ProductCategory
            {
                CategoryId = categoryId
            };
            product.AddCategory(productCategory);
        }

        var deletedCategories = product.Categories.Where(p => !request.CategoryIds.Contains(p.CategoryId)).ToList();

        foreach (var deletedCategory in deletedCategories)
        {
            deletedCategory.Product = null;
            product.Categories.Remove(deletedCategory);
            _dbContext.Set<ProductCategory>().Remove(deletedCategory);
        }
    }

    private async void AddOrDeleteProductOption(IList<UpdateProductOptionDto> updateProductOptionDtos, Product product)
    {
        foreach (var option in updateProductOptionDtos)
        {
            var optionValue = product.OptionValues.FirstOrDefault(x => x.ProductOptionId == option.OptionId);
            if (optionValue == null)
            {
                await AddOptionToProduct(product.ProductId, (int)option.OptionId!,
                    JsonSerializer.Deserialize<List<string>>(option.Value)!);
            }
            else
            {
                optionValue.Value = option.Value;
            }
        }
        
        var deletedProductOptionValues = product.OptionValues.Where(x => updateProductOptionDtos.All(po => x.ProductOptionId != po.OptionId)).ToList();

        foreach (var productOptionValue in deletedProductOptionValues)
        {
            product.OptionValues.Remove(productOptionValue);
            _dbContext.Set<ProductOptionValue>().Remove(productOptionValue);
        }
        await _dbContext.SaveChangesAsync();
    }     
}