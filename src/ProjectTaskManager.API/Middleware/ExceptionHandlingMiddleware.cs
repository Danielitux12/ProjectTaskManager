using System.Net;
using System.Text.Json;
using ProjectTaskManager.Application.Exceptions;

namespace ProjectTaskManager.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado procesando {Path}", context.Request.Path);
            await ManejarExcepcionAsync(context, ex);
        }
    }

    private static Task ManejarExcepcionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, titulo) = ex switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
            BusinessRuleException => (HttpStatusCode.Conflict, "Regla de negocio violada"),
            ArgumentException => (HttpStatusCode.BadRequest, "Solicitud inválida"),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        var problemDetails = new
        {
            title = titulo,
            status = (int)statusCode,
            detail = ex.Message
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}