using Electronic.Persistence.DatabaseContext;
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

        return services;
    }
}