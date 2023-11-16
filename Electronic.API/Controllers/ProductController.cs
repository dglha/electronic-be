using Electronic.API.Requests;
using Electronic.Application.Contracts.Common;
using Electronic.Application.Contracts.DTOs.Product;
using Electronic.Application.Contracts.DTOs.ProductOption;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Electronic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Create Product.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateProduct([FromForm] CreateProductRequestForm request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newRequest = new CreateProductDto
            {
                Name = request.Name,
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
                IsPublished = (bool)request.IsPublished!,
                IsFeatured = (bool)request.IsFeatured!,
                IsAllowToOrder = (bool)request.IsAllowToOrder!,
                BrandId = (int)request.BrandId!,
                ThumbnailImage = new ImageFileDto
                {
                    FileName = request.ThumbnailImage.FileName,
                    FileType = request.ThumbnailImage.ContentType,
                    FileContent = request.ThumbnailImage.OpenReadStream()
                },
                ProductImages = request.ProductImages.Select(i => new ImageFileDto
                {
                    FileName = i.FileName,
                    FileType = i.ContentType,
                    FileContent = i.OpenReadStream()
                }),
                CategoryIds = request.CategoryIds,
            };
            await _productService.CreateProduct(newRequest);
            return Ok();
        }

        /// <summary>
        /// Update product info
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{productId:long}/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateProduct(long productId, [FromForm] UpdateProductRequestForm request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newRequest = new UpdateProductDto
            {
                Name = request.Name,
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
                IsPublished = (bool)request.IsPublished!,
                IsFeatured = (bool)request.IsFeatured!,
                IsAllowToOrder = (bool)request.IsAllowToOrder!,
                BrandId = (int)request.BrandId!,
                ThumbnailImage = new ImageFileDto
                {
                    FileName = request.ThumbnailImage.FileName,
                    FileType = request.ThumbnailImage.ContentType,
                    FileContent = request.ThumbnailImage.OpenReadStream()
                },
                ProductImages = request.ProductImages.Select(i => new ImageFileDto
                {
                    FileName = i.FileName,
                    FileType = i.ContentType,
                    FileContent = i.OpenReadStream()
                }),
                CategoryIds = request.CategoryIds,
                DeletedMediaIds = request.DeletedMediaIds,
            };
            await _productService.UpdateProduct(productId, newRequest);
            return Ok();
        }

        /// <summary>
        /// Add option to product.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{productId:long}/options")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddProductOption(long productId, [FromForm] AddProductOptionValueDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _productService.AddOptionToProduct(productId, request.OptionId, request.Values!);
            return Ok();
        }

        /// <summary>
        /// Update product's option
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{productId:long}/options-update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateProductOption(long productId, IEnumerable<UpdateProductOptionDto> request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _productService.UpdateProductOption(productId, request.ToList());
            return Ok();
        }


        /// <summary>
        /// Add product variant
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost("{productId:long}/variants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddProductVariant(long productId,
            [FromForm] CreateProductVariantRequestForm request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newRequest = new CreateProductVariantDto
            {
                Name = request.Name,
                Slug = request.Slug,
                SKU = request.SKU,
                Price = request.Price,
                OldPrice = request.OldPrice,
                OptionCombinations = request.OptionCombinations,
                ThumbnailImage = new ImageFileDto
                {
                    FileName = request.ThumbnailImage.FileName,
                    FileType = request.ThumbnailImage.ContentType,
                    FileContent = request.ThumbnailImage.OpenReadStream()
                },
                NewImages = request.NewImages.Select(i => new ImageFileDto
                {
                    FileName = i.FileName,
                    FileType = i.ContentType,
                    FileContent = i.OpenReadStream()
                }),
            };

            await _productService.AddProductVariant(productId, newRequest);
            return Ok();
        }

        /// <summary>
        /// Update product's variants
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{productId:long}/variants-update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateProductVariant(long productId,
            IEnumerable<UpdateProductVariantDto> request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _productService.UpdateProductVariant(productId, request);
            return Ok();
        }

        /// <summary>
        /// Get product list 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("product-list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pagination<ProductListDto>>> GetProductList(int pageNumber = 1, int take = 20)
        {
            return Ok(await _productService.GetProductList(pageNumber, take));
        }
        
        /// <summary>
        /// Get product detail
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("{productId:long}/detail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<ProductDetailDto>>> GetProductDetail(long productId)
        {
            return Ok(await _productService.GetProductDetail(productId));
        }
        
        /// <summary>
        /// Update product price
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{productId:long}/update-price")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseResponse<ProductDetailDto>>> UpdateProductPrice(long productId, [FromForm] UpdateProductPriceRequestDto request)
        {
            return Ok(await _productService.UpdateProductPrice(productId, request));
        }
        
        /// <summary>
        /// Get product's options value
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpGet("{productId:long}/option-values")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ProductOptionValueDto>>> GetProductOptionValues(long productId)
        {
            return Ok(await _productService.GetProductOptionsDetail(productId));
        }
    }
}