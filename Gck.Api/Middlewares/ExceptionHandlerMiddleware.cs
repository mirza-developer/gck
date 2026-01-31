using System.Net;
using System.Text.Json;

namespace Gck.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Generate correlation ID for tracking
        var correlationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

        // Log the exception with details
        _logger.LogError(exception,
            "An unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}",
            correlationId,
            context.Request.Path);

        // Determine status code based on exception type
        var statusCode = exception switch
        {
            ArgumentNullException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            NotImplementedException => HttpStatusCode.NotImplemented,
            InvalidOperationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        // Create response object
        var response = new ErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = GetTitle(statusCode),
            Status = (int)statusCode,
            Detail = _environment.IsDevelopment() ? exception.Message : "An error occurred while processing your request.",
            Instance = context.Request.Path,
            CorrelationId = correlationId
        };

        // Add stack trace in development mode
        if (_environment.IsDevelopment())
        {
            response.Extensions = new Dictionary<string, object?>
            {
                { "exceptionType", exception.GetType().Name },
                { "stackTrace", exception.StackTrace },
                { "innerException", exception.InnerException?.Message }
            };
        }

        // Set response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }

    private static string GetTitle(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Bad Request",
            HttpStatusCode.Unauthorized => "Unauthorized",
            HttpStatusCode.Forbidden => "Forbidden",
            HttpStatusCode.NotFound => "Not Found",
            HttpStatusCode.NotImplemented => "Not Implemented",
            HttpStatusCode.InternalServerError => "Internal Server Error",
            _ => "An error occurred"
        };
    }
}

public class ErrorResponse
{
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Detail { get; set; } = string.Empty;
    public string Instance { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public Dictionary<string, object?>? Extensions { get; set; }
}
