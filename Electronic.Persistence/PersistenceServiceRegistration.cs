using Electronic.Application.Contracts.Persistences;
using Electronic.Application.Interfaces.Services;
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

        #endregion

        #region DI Services

        services.AddScoped<IBrandService, BrandService>();

        #endregion
        
        return services;
    }
}