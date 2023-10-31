using Electronic.Application.Contracts.Persistences;
using Electronic.Application.Interfaces.Persistences;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Models.Catalog;
using Electronic.Persistence.DatabaseContext;
using Electronic.Persistence.Implements.Services;
using Electronic.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Electronic.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ElectronicDatabaseContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Electronic"));
        });

        // DI

        #region DI Repository
        
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<INewCategoryRepository, NewCategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductOptionRepository, ProductOptionRepository>();
        
        #endregion

        #region DI Services

        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<INewService, NewService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductOptionService, ProductOptionService>();

        #endregion
        
        return services;
    }
}