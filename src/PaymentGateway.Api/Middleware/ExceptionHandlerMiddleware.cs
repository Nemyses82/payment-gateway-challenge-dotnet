using System.Net;
using System.Text.Json;

namespace PaymentGateway.Api.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            var errorResponse = MapErrorResponse(context, e.Message);
            logger.LogError(e, e.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    private static ErrorResponse MapErrorResponse(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";
        return new ErrorResponse((HttpStatusCode)context.Response.StatusCode, null, message, false);
    }
}

internal record ErrorResponse(HttpStatusCode StatusCode, object? Data, string Message, bool Success);