using Electronic.Application.Contracts.Logging;
using Electronic.Application.Interfaces.Services;
using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddSingleton<IStorageService, LocalStorageService.LocalStorageService>();
        return services;
    }
}