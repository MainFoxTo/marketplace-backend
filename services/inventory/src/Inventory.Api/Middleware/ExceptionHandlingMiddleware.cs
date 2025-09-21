using Inventory.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Inventory.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InsufficientStockException ex)
            {
                _logger.LogWarning(ex, "Insufficient stock: {ProductId}", ex.ProductId);
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new { message = ex.Message, ex.ProductId, ex.RequestedQuantity, ex.AvailableQuantity });
                await context.Response.WriteAsync(payload);
            }
            catch (UnauthorizedAccessException ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new { message = ex.Message });
                await context.Response.WriteAsync(payload);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var payload = JsonSerializer.Serialize(new { message = "Internal server error" });
                await context.Response.WriteAsync(payload);
            }
        }
    }
}
