using System.Net;
using System.Text.Json;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.Product.User;
using Electronic.Application.Contracts.DTOs.Review;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Identity;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Queries;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Catalog;
using Electronic.Domain.Models.Core;
using Electronic.Domain.Models.Inventory;
using Electronic.Domain.Models.Review;
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
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public ProductService(ElectronicDatabaseContext dbContext, IProductRepository productRepository,
        IMediaService mediaService, IProductOptionRepository productOptionRepository, IAppLogger<ProductService> logger, IUserService userService, IAuthService authService)
    {
        _dbContext = dbContext;
        _productRepository = productRepository;
        _mediaService = mediaService;
        _productOptionRepository = productOptionRepository;
        _logger = logger;
        _userService = userService;
        _authService = authService;
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
            IsVisibleIndividually = true,
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

        await SaveProductMedias(request, product);
        
        // Create product Stock
        if (product.StockQuantity == null || product.StockQuantity == 0)
        {
            var warehouse = await _dbContext.Set<Warehouse>().FirstAsync();
            var stock = new Stock
            {
                Product = product,
                Quantity = 0,
                Warehouse = warehouse,
            };
            var stockHistory = new StockHistory
            {
                Note = StockHistoryNoteEnum.Create.ToString(),
                Stock = stock,
                AdjustedQuantity = stock.Quantity,
                OldQuantity = 0,
            };
            _dbContext.Set<Stock>().Add(stock);
            _dbContext.Set<StockHistory>().Add(stockHistory);
        }


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
            .Include(c => c.Categories)
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

        // Add option group (Combination)
        foreach (var optionCombination in request.OptionCombinations)
        {
            var productOption = await _dbContext.Set<ProductOptionValue>()
                .Where(option => option.ProductOptionId == optionCombination.ProductOptionId && option.ProductId == product.ProductId).FirstAsync();

            if (productOption is null)
                throw new AppException("Product Option not found!", (int)HttpStatusCode.BadRequest);

            var values = JsonSerializer.Deserialize<List<string>>(productOption.Value);

            if (values is null || values.Count == 0)
                throw new AppException("Product option not contain any value", (int)HttpStatusCode.BadRequest);

            if (!values.Contains(optionCombination.Value))
                throw new AppException("Product option value not valid", (int)HttpStatusCode.BadRequest);

            variantProduct.AddOptionCombination(new ProductOptionGroup
            {
                ProductOptionId = (int)optionCombination.ProductOptionId!,
                Value = optionCombination.Value,
            });
        }

        // Upload image
        if (product.ThumbnailImage != null)
        {
            variantProduct.ThumbnailImage = new Media
                { FileName = product.ThumbnailImage.FileName, Caption = "", MediaType = MediaTypeEnum.Image };
        }

        await MapProductVariantImageFromRequest(request, variantProduct);

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
        
        // Create product Stock
        if (variantProduct.StockQuantity != null)
        {
            var warehouse = await _dbContext.Set<Warehouse>().FirstAsync();
            var stock = new Stock
            {
                Product = product,
                Quantity = (int)variantProduct.StockQuantity,
                Warehouse = warehouse,
            };
            var stockHistory = new StockHistory
            {
                Note = StockHistoryNoteEnum.Create.ToString(),
                Stock = stock,
                AdjustedQuantity = stock.Quantity,
                OldQuantity = 0,
            };
            
            _dbContext.Set<Stock>().Add(stock);
            _dbContext.Set<StockHistory>().Add(stockHistory);
        }

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
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null) throw new AppException("Product doesn't exists", (int)HttpStatusCode.BadRequest);

        var isPriceChanged = product.Price != request.Price ||
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
        product.OldPrice = product.Price;
        product.Price = request.Price;
        product.SpecialPriceStartDate = request.SpecialPriceStartDate;
        product.SpecialPriceEndDate = request.SpecialPriceEndDate;
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

        await _dbContext.SaveChangesAsync();
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

    public async Task<Pagination<ProductListDto>> GetProductList(int pageIndex, int itemPerPage)
    {
        var query = _dbContext.Products.Include(p => p.Brand).AsQueryable();
        var totalCount = await query.CountAsync();
        var data = await query.Skip((pageIndex - 1) * itemPerPage).Take(itemPerPage)
            .Select(p => new ProductListDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Slug = p.Slug,
                IsPublished = p.IsPublished,
                IsFeatured = p.IsFeatured,
                IsAllowToOrder = p.IsAllowToOrder,
                SpecialPrice = p.SpecialPrice,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                IsVisibleIndividually = p.IsVisibleIndividually,
                HasOption = p.HasOption,
                Brand = p.Brand.Name
            }).ToListAsync();
        return Pagination<ProductListDto>.ToPagination(data, pageIndex, itemPerPage, totalCount);
    }

    public async Task<BaseResponse<ProductDetailDto>> GetProductDetail(long productId)
    {
        var product = await _dbContext.Set<Product>()
            .Include(p => p.ThumbnailImage)
            .Include(p => p.Categories).ThenInclude(c => c.Category)
            .Include(p => p.OptionValues).ThenInclude(productOptionValue => productOptionValue.ProductOption)
            .Include(product => product.Medias).ThenInclude(productMedia => productMedia.Media)
            .Include(p => p.OptionCombinations).ThenInclude(productOptionGroup => productOptionGroup.ProductOption)
            .Include(p => p.Brand)
            .Include(p => p.ProductLinks)
            .Where(p => p.ProductId == productId).FirstOrDefaultAsync();

        if (product == null) throw new AppException("Product not found", 404);

        var hasParentProduct = _dbContext.ProductLinks.Any(link =>
            link.LinkedProductId == product.ProductId && link.Type == ProductLinkEnum.Variant);

        var variantProductIds = product.ProductLinks.Select(x => x.LinkedProductId).ToList();

        var variantProducts = await _dbContext.Set<Product>()
            .Include(p => p.OptionCombinations).ThenInclude(productOptionGroup => productOptionGroup.ProductOption)
            .Where(p => variantProductIds.Contains(p.ProductId)).ToListAsync();
        
        var productDto = new ProductDetailDto
        {
            Name = product.Name,
            Slug = product.Slug,
            IsPublished = product.IsPublished,
            IsFeatured = product.IsFeatured,
            IsAllowToOrder = product.IsAllowToOrder,
            BrandId = product.BrandId,
            HasOption = product.HasOption,
            IsDeleted = product.IsDeleted,
            Description = product.Description,
            Specification = product.Specification,
            Price = product.Price,
            OldPrice = product.OldPrice,
            SpecialPrice = product.SpecialPrice,
            SpecialPriceStartDate = product.SpecialPriceStartDate,
            SpecialPriceEndDate = product.SpecialPriceEndDate,
            SKU = product.SKU,
            ShortDescription = product.ShortDescription,
            ProductId = product.ProductId,
            StockQuantity = product.StockQuantity,
            IsVisibleIndividually = product.IsVisibleIndividually,
            Brand = product.Brand.Name,
            NormalizedName = product.NormalizedName,
            IsNewProduct = product.IsNewProduct,
            ThumbnailImageUrl = product.ThumbnailImage != null ? _mediaService.GetThumbnailUrl(product.ThumbnailImage) : "",
            HasParentProduct = hasParentProduct,
            ProductLinks = product.ProductLinks.Select(l => new ProductLinkDto
            {
                ProductId = l.LinkedProductId,
                LinkType = l.Type.ToString()
            }).ToList(),
            Categories = product.Categories.Select(c => new ProductCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.Category.Name
            }).ToList(),
            OptionValues = product.OptionValues.Select(o => new ProductOptionValueDto
            {
                ProductOptionId = o.ProductOptionId,
                ProductOption = o.ProductOption.Name,
                Value = JsonSerializer.Deserialize<List<string>>(o.Value),
            }),
            OptionCombinations = product.OptionCombinations.Select(oc => new ProductOptionCombinationDto
            {
                ProductOptionId = oc.ProductOptionId,
                ProductOption = oc.ProductOption.Name,
                Value = oc.Value,
            }),
            MediasUrl = product.Medias.Select(m => new ProductMediaDto
                { MediaUrl = _mediaService.GetMediaUrl(m.Media), MeidaId = m.MediaId }),
            ProductVariants = hasParentProduct ? 
                new List<ProductDetailDto>() :
                variantProducts.Select(p => new ProductDetailDto
                {
                    Name = p.Name,
                    SKU = p.SKU,
                    ThumbnailImageUrl = _mediaService.GetThumbnailUrl(p.ThumbnailImage),
                    OptionCombinations = p.OptionCombinations.Select(oc => new ProductOptionCombinationDto
                    {
                        ProductOptionId = oc.ProductOptionId,
                        ProductOption = oc.ProductOption.Name,
                        Value = oc.Value,
                    }),
                    StockQuantity = p.StockQuantity
                })
        };

        return new BaseResponse<ProductDetailDto>(productDto);
    }

    public async Task<BaseResponse<string>> UpdateProductPrice(long productId, UpdateProductPriceRequestDto request)
    {
        var product = await _dbContext.Set<Product>().FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product is null) throw new AppException("Product not found", 400);

        var isPriceChanged = product.Price != request.Price ||
                             product.SpecialPrice != request.SpecialPrice ||
                             product.SpecialPriceStartDate != request.SpecialPriceStartDate ||
                             product.SpecialPriceEndDate != request.SpecialPriceEndDate;

        if (!isPriceChanged)
            return new BaseResponse<string>
            {
                Data = "",
                Message = "Ok",
                IsSuccess = true
            };
        product.OldPrice = product.Price;
        product.Price = request.Price;
        product.SpecialPrice = request.SpecialPrice;
        product.SpecialPriceStartDate = request.SpecialPriceStartDate;
        product.SpecialPriceEndDate = request.SpecialPriceEndDate;

        var productPriceHistory = CreatePriceHistory(product);
        product.PriceHistories.Add(productPriceHistory);

        await _dbContext.SaveChangesAsync();

        return new BaseResponse<string>
        {
            Data = "",
            Message = "Updated",
            IsSuccess = true
        };
    }

    public async Task<IEnumerable<ProductOptionValueDto>> GetProductOptionsDetail(long productId)
    {
        var product = await _dbContext.Set<Product>().Include(product => product.OptionValues)
            .ThenInclude(productOptionValue => productOptionValue.ProductOption)
            .FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product is null) throw new AppException("Product not found", (int)HttpStatusCode.BadRequest);

        var result = product.OptionValues.Select(o => new ProductOptionValueDto
        {
            ProductOption = o.ProductOption.Name,
            Value = JsonSerializer.Deserialize<List<string>>(o.Value),
        }).ToList();

        return result;
    }

    public async Task<BaseResponse<IEnumerable<ProductUserDto>>> GetFeaturedProducts()
    {
        var featuredProducts = await _dbContext.Set<Product>()
            .Where(p => !p.IsDeleted && p.IsVisibleIndividually && p.IsFeatured && p.StockQuantity > 0).OrderByDescending(p => p.UpdatedAt).Take(4).Select(p =>
                new ProductUserDto
                {
                    Name = p.Name,
                    ProductId = p.ProductId,
                    Slug = p.Slug,
                    SpecialPrice = DateTime.Now > p.SpecialPriceEndDate ? null : p.SpecialPrice,
                    Price = p.Price,
                    ThumbnailImage = _mediaService.GetThumbnailUrl(p.ThumbnailImage)
                }).ToListAsync();

        return new BaseResponse<IEnumerable<ProductUserDto>>(featuredProducts);
    }

    public async Task<BaseResponse<IEnumerable<ProductUserDto>>> GetNewProducts()
    {
        var newProducts = await _dbContext.Set<Product>()
            .Where(p => !p.IsDeleted && p.IsVisibleIndividually && p.IsNewProduct && p.StockQuantity > 0).OrderByDescending(p => p.UpdatedAt).Take(4).Select(p =>
                new ProductUserDto
                {
                    Name = p.Name,
                    ProductId = p.ProductId,
                    Slug = p.Slug,
                    SpecialPrice = DateTime.Now > p.SpecialPriceEndDate ? null : p.SpecialPrice,
                    Price = p.Price,
                    ThumbnailImage = _mediaService.GetThumbnailUrl(p.ThumbnailImage)
                }).ToListAsync();

        return new BaseResponse<IEnumerable<ProductUserDto>>(newProducts);
    }

    public async Task<BaseResponse<ProductDetailUserDto>> GetProductUserDetail(long productId)
    {
        var isVariant = _dbContext.Set<ProductLink>()
            .Any(p => p.Type == ProductLinkEnum.Variant && p.LinkedProductId == productId);

        if (isVariant)
            return new BaseResponse<ProductDetailUserDto>
            {
                Data = null,
                IsSuccess = false,
                Message = "Product not found"
            };
        
        var product = await _dbContext.Set<Product>().Where(p => p.ProductId == productId)
            .Include(product => product.Brand).Include(product => product.ThumbnailImage)
            .Include(product => product.Categories).ThenInclude(productCategory => productCategory.Category)
            .Include(product => product.Medias).ThenInclude(productMedia => productMedia.Media)
            .Include(product => product.OptionValues)
            .ThenInclude(productOptionValue => productOptionValue.ProductOption).FirstOrDefaultAsync();

        if (product is null) return new BaseResponse<ProductDetailUserDto>
        {
            Data = null,
            IsSuccess = false,
            Message = "Product not found"
        };

        var variantIds = await _dbContext.Set<ProductLink>().Where(p => p.Type == ProductLinkEnum.Variant && p.ProductId == product.ProductId).Select(p => p.LinkedProductId).ToListAsync();

        var productVariants = await _dbContext.Set<Product>()
            .Include(product => product.Brand).Include(product => product.ThumbnailImage)
            .Include(product => product.Categories).ThenInclude(productCategory => productCategory.Category)
            .Include(product => product.Medias).ThenInclude(productMedia => productMedia.Media)
            .Include(product => product.OptionValues)
            .ThenInclude(productOptionValue => productOptionValue.ProductOption)
            .Where(p => variantIds.Count != 0 && variantIds.Contains(p.ProductId))
            .Select(p => new ProductDetailUserDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Slug = p.Slug,
                SpecialPrice = p.SpecialPrice,
                Price = p.Price,
                SpecialPriceEndDate = p.SpecialPriceEndDate,
                SpecialPriceStartDate = p.SpecialPriceStartDate,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                OptionCombinations = p.OptionCombinations.Select(oc => new ProductOptionCombinationDto
                {
                    ProductOptionId = oc.ProductOptionId,
                    ProductOption = oc.ProductOption.Name,
                    Value = oc.Value,
                }),
                ThumbnailImageUrl = _mediaService.GetThumbnailUrl(p.ThumbnailImage)
            }).ToListAsync();

        var productVariantImages = productVariants.Select(o => o.ThumbnailImageUrl).ToList();
        
        
        var productDto = new ProductDetailUserDto
        {
            Name = product.Name,
            Slug = product.Slug,
            SpecialPrice = product.SpecialPrice,
            Price = product.Price,
            ProductId = product.ProductId,
            Brand = product.Brand.Name,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            Categories = product.Categories.Select(c => new ProductCategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.Category.Name
            }),
            MediasUrl = product.Medias.Select(m => _mediaService.GetMediaUrl(m.Media)).ToList(),
            BrandId = product.BrandId,
            SKU = product.SKU,
            ThumbnailImageUrl = _mediaService.GetThumbnailUrl(product.ThumbnailImage),
            OptionValues = product.OptionValues.Select(o => new ProductOptionValueDto
            {
                ProductOptionId = o.ProductOptionId,
                ProductOption = o.ProductOption.Name,
                Value = JsonSerializer.Deserialize<List<string>>(o.Value),
            }),
            StockQuantity = product.StockQuantity,
            SpecialPriceEndDate = product.SpecialPriceEndDate,
            SpecialPriceStartDate = product.SpecialPriceStartDate,
            ProductVariants = productVariants
        };
        
        productDto.MediasUrl.AddRange(productVariantImages);

        return new BaseResponse<ProductDetailUserDto>(productDto);
    }

    public async Task<Pagination<ProductUserDto>> GetProductList(ProductQuery query)
    {
        var productQuery = GetProductUserQuery();

        if (query.CategoryId.HasValue && query.CategoryId > 0)
        {
            productQuery = productQuery.Where(p => p.Categories.Any(c => c.CategoryId == query.CategoryId));
        }

        if (query.BrandId.HasValue && query.BrandId > 0)
        {
            productQuery = productQuery.Where(p => p.BrandId.HasValue && p.BrandId == query.BrandId);
        }

        if (query.MinPrice.HasValue && query.MinPrice > 0)
        {
            productQuery = productQuery.Where(p =>
                p.SpecialPriceEndDate <= DateTime.Now ? p.Price >= query.MinPrice : p.SpecialPrice >= query.MinPrice);
        }
        
        if (query.MaxPrice.HasValue && query.MaxPrice > 0)
        {
            productQuery = productQuery.Where(p =>
                p.SpecialPriceEndDate <= DateTime.Now ? p.Price <= query.MaxPrice : p.SpecialPrice <= query.MinPrice);
        }
        
        if (!string.IsNullOrEmpty(query.Name))
        {
            productQuery = productQuery.Where(p => p.Name.Contains(query.Name));
        }
        
        if (!query.Page.HasValue && query.Page < 0)
        {
            query.Page = 1;
        }

        if (!query.PageSize.HasValue && query.PageSize < 0)
        {
            query.Page = 15;
        }

        if (!string.IsNullOrEmpty(query.SortBy) && query.SortBy.Equals("price") && query.IsSortAscending.HasValue)
        {
            productQuery = (bool)query.IsSortAscending
                ? productQuery.OrderBy(p => p.Price)
                : productQuery.OrderByDescending(p => p.Price);
        }

        var totalCount = await productQuery.CountAsync();
        var data = await productQuery.Skip((int)(query.Page! - 1) * (int)query.PageSize).Take((int)query.PageSize!).Select(p => new ProductUserDto
        {
            Name = p.Name,
            ProductId = p.ProductId,
            Slug = p.Slug,
            SpecialPrice = DateTime.Now > p.SpecialPriceEndDate ? null : p.SpecialPrice,
            Price = p.Price,
            ThumbnailImage = _mediaService.GetThumbnailUrl(p.ThumbnailImage)
        }).ToListAsync();

        return Pagination<ProductUserDto>.ToPagination(data, (int)query.Page, (int)query.PageSize, totalCount);
    }

    public async Task<IEnumerable<ProductPriceHistoryDto>> GetProductPriceHistory(long productId)
    {
        var product = await _dbContext.Set<Product>().Where(p => p.ProductId == productId).FirstOrDefaultAsync();

        if (product is null) throw new AppException("Product not found!", (int)HttpStatusCode.BadRequest);

        var priceHistories = await _dbContext.Set<ProductPriceHistory>().Where(p => p.ProductId == product.ProductId)
            .Select(p => new ProductPriceHistoryDto
            {
                ProductId = p.ProductId,
                OldPrice = p.OldPrice,
                SpecialPrice = p.SpecialPrice,
                Price = p.SpecialPrice,
                SpecialPriceEndDate = p.SpecialPriceEndDate,
                SpecialPriceStartDate = p.SpecialPriceStartDate
            }).ToListAsync();

        return priceHistories;
    }

    public async Task AddProductReview(long productId, string userEmail,ProductReviewRequestDto request)
    {
        var product = await _dbContext.Set<Product>().Where(p => p.ProductId == productId).FirstOrDefaultAsync();

        if (product is null) throw new AppException("Product not found!", (int)HttpStatusCode.BadRequest);

        var user = await _authService.GetCurrentUserInfo(userEmail);

        var review = new Review
        {
            Comment = request.Comment,
            Rating = request.Rating,
            ReviewerName = user.Username,
            ProductId = productId,
            ReviewerId = _userService.UserId
        };

        _dbContext.Set<Review>().Add(review);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Pagination<ProductReviewDto>> GetProductReview(long productId, int pageNumber, int itemPerPage)
    {
        var query = _dbContext.Set<Review>().Where(p => p.ProductId == productId);
        var totalCount = await query.CountAsync();
        if (pageNumber == -1)
        {
            itemPerPage = totalCount;
            pageNumber *= -1;
        }

        var data = await query.Skip((pageNumber - 1) * itemPerPage).Take(itemPerPage).Select(s =>
            new ProductReviewDto
            {
                Comment = s.Comment,
                Rating = s.Rating,
                ReviewerName = s.ReviewerName,
                ReviewId = s.ReviewId,
            }).ToListAsync();
        return Pagination<ProductReviewDto>.ToPagination(data, pageNumber, itemPerPage, totalCount);
    }

    private IQueryable<Product> GetProductUserQuery()
    {
        return _dbContext.Set<Product>().Where(p => !p.IsDeleted && p.IsVisibleIndividually);
    }

    // public async Task GetAvailableOptionCombination(long productId)
    // {
    //     var product = await _dbContext.Set<Product>().Where(p => p.ProductId == productId).FirstOrDefaultAsync();
    //
    //     if (product is null) throw new AppException("Product not found", (int)HttpStatusCode.BadRequest);
    //
    //     var productVariantIds = await _dbContext.Set<ProductLink>()
    //         .Where(p => p.ProductId == product.ProductId && p.Type == ProductLinkEnum.Variant)
    //         .Select(p => p.LinkedProductId).ToListAsync();
    //     
    //     var productOptions =
    //         await _dbContext.ProductOptionValues.Where(o => o.ProductId == product.ProductId).ToListAsync();
    //     
    //     var usedCombinations = _dbContext.Set<ProductOptionGroup>().Where(variant => productVariantIds.Contains(variant.ProductId) && )
    //
    //
    //     var a = productOptions.Select(x => new ProductOptionValueDto
    //     {
    //         ProductOptionId = x.ProductOptionId,
    //         Value = JsonSerializer.Deserialize<List<string>>(x.Value)
    //     });
    // }

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
    
    private async Task SaveProductMedias(UpdateProductDto request, Product product)
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

        if (request.ProductImages is null || !request.ProductImages.Any()) return;
        
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

        var deletedProductOptionValues = product.OptionValues
            .Where(x => updateProductOptionDtos.All(po => x.ProductOptionId != po.OptionId)).ToList();

        foreach (var productOptionValue in deletedProductOptionValues)
        {
            product.OptionValues.Remove(productOptionValue);
            _dbContext.Set<ProductOptionValue>().Remove(productOptionValue);
        }

        await _dbContext.SaveChangesAsync();
    }
}