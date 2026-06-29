using System.Net;
using System.Text.Json;
using BrbHabitaciones.Application.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace BrbHabitaciones.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the full chain so EasyPanel logs show the real PostgreSQL error
            logger.LogError(ex,
                "Error no manejado [{Type}] en {Method} {Path}: {Message}",
                ex.GetType().Name,
                context.Request.Method,
                context.Request.Path,
                ex.Message);

            if (ex.InnerException is not null)
                logger.LogError("Inner exception [{Type}]: {Message}",
                    ex.InnerException.GetType().Name,
                    ex.InnerException.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        if (context.Response.HasStarted) return;

        context.Response.ContentType = "application/json";

        var (statusCode, message) = ex switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
            InvalidOperationException   => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentException           => (HttpStatusCode.BadRequest, ex.Message),
            KeyNotFoundException        => (HttpStatusCode.NotFound, ex.Message),
            DbUpdateException dbEx      => (HttpStatusCode.InternalServerError,
                // Only expose the inner message in Development; generic message in Production
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? $"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}"
                    : "Error al procesar la solicitud. Intentá de nuevo más tarde."),
            _ => (HttpStatusCode.InternalServerError, "Ocurrió un error interno. Intentá de nuevo más tarde.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
