using Microsoft.AspNetCore.Authorization;

namespace CRUDMVC.Middlewares;

public class CustomExceptionHandlingMiddleware
{
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;
    public CustomExceptionHandlingMiddleware(ILogger<CustomExceptionHandlingMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.InnerException.GetType().ToString(), ex.InnerException.Message);
            }
            else
            {
                _logger.LogError("{ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error occurred");
        }

    }
}

public static class CustomExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlingMiddleware>();
    }
}