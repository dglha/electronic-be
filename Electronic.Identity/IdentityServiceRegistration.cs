using System.Text;
using Electronic.Application.Contracts.Identity;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Identity;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddIdentityService(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        serviceCollection.AddDbContext<ElectronicIdentityDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ElectronicIdentity"));
        });

        serviceCollection.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ElectronicIdentityDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddDefaultTokenProviders();
        
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
            };
        });
        
        // DI
        serviceCollection.AddTransient<IAuthService, AuthService>();
        serviceCollection.AddTransient<IUserService, UserService>();
        
        return serviceCollection;
    }
}