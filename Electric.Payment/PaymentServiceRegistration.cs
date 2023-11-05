using Electric.Payment.VNPay.Config;
using Electric.Payment.VNPay.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Electric.Payment;

public static class PaymentServiceRegistration
{
    public static IServiceCollection AddPaymentService(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.Configure<VnPayConfig>(configuration.GetSection("VnPay"));

        serviceCollection.AddTransient<IVnPayPaymentService, VnPayPaymentService>();
        
        return serviceCollection;
    }
}