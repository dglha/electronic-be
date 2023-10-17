using System.Net;
using System.Text.Json;
using Electronic.API.Models;
using Electronic.Application.Exceptions;

namespace Electronic.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    private readonly IHostEnvironment _host;

    public ExceptionMiddleware(RequestDelegate next, IHostEnvironment host)
    {
        _next = next;
        _host = host;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            // await HandleExceptionAsync(httpContext, e);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var res = _host.IsDevelopment()
                ? new ApiException((int)HttpStatusCode.InternalServerError, e.Message, e.StackTrace.ToString())
                : new ApiException((int)HttpStatusCode.InternalServerError);
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(res, options);
            await httpContext.Response.WriteAsync(json);
        }
    }
}