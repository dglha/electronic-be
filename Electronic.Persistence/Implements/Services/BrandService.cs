using System.Net;
using Electronic.Application.Contracts.DTOs.Brand;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Persistences;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Model.Catalog;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IAppLogger<BrandService> _logger;
    private readonly ElectronicDatabaseContext _dbContext;

    public BrandService(IBrandRepository brandRepository, IAppLogger<BrandService> logger,
        ElectronicDatabaseContext dbContext)
    {
        _brandRepository = brandRepository;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<CreateBrandResultDto> CreateBrand(CreateBrandDto request)
    {
        var slug = SlugGenerator.Generate(request.Name);
        var safeSlug = _brandRepository.ConvertToSafeSlug(slug);
        var brand = new Brand
        {
            Name = request.Name,
            Description = request.Description,
            Slug = safeSlug,
            IsPublished = (bool)request.IsPublished!,
            IsDeleted = false // When create new -> not delete
        };

        await _brandRepository.CreateAsync(brand);
        return new CreateBrandResultDto
        {
            Name = brand.Name,
            Slug = brand.Slug,
            Description = brand.Description
        };
    }

    public async Task TogglePublishBrand(int brandId)
    {
        var brand = await _dbContext.Set<Brand>().FirstOrDefaultAsync(b => b.BrandId == brandId && !b.IsDeleted);
        if (brand == null) throw new AppException("Brand Id not found", (int)HttpStatusCode.BadRequest);
        brand.IsPublished = !brand.IsPublished;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Pagination<BrandDto>> GetAvailableBrands(int pageNumber, int itemPerPage)
    {
        var query = GetListAvailableBrandQuery();
        var totalCount = await query.CountAsync();
        var data = await query.Skip((pageNumber - 1) * itemPerPage).Take(itemPerPage).ToListAsync();
        return Pagination<BrandDto>.ToPagination(data, pageNumber, itemPerPage, totalCount);
    }

    public async Task DeleteBrand(int brandId)
    {
        var brand = await _dbContext.Set<Brand>().FirstOrDefaultAsync(b => b.BrandId == brandId && !b.IsDeleted);
        if (brand == null) throw new AppException("Brand Id not found", (int)HttpStatusCode.BadRequest);
        brand.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
    }

    private IQueryable<BrandDto> GetListAvailableBrandQuery()
        => _dbContext.Set<Brand>().Where(b => !b.IsDeleted)
            .AsNoTracking()
            .Select(b => new BrandDto
            {
                BrandId = b.BrandId,
                Name = b.Name,
                Description = b.Description,
                Slug = b.Slug,
                IsPublished = b.IsPublished
            }).AsQueryable();
}