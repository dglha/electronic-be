using System.Reflection;
using Electric.Payment;
using Electronic.API.Middlewares;
using Electronic.Application.Exceptions;
using Electronic.Persistence;
using Identity;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


#region Serilog

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration));

#endregion


// Add services to the container.

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPaymentService(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

// Adding custom error response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();
        return new BadRequestObjectResult(new ApiValidationErrorResponse() { Errors = errors });
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo() { Title = "MyAPI", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

#region CORS

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            .WithOrigins("https://localhost:4200"); 
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200");
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://vanhieustore.com:4200");
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://vanhieustore.com:4200");
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://91ea-2402-800-629c-7407-e912-dc49-6c79-1a28.ngrok-free.app");
    });
});

#endregion

var app = builder.Build();

#region Middlewares

app.UseMiddleware<ExceptionMiddleware>();

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();