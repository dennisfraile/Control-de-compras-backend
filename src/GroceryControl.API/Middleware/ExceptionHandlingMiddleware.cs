using System.Net;

namespace GroceryControl.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex, _env);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case Application.Common.Exceptions.ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "One or more validation failures have occurred.",
                    statusCode = (int)HttpStatusCode.BadRequest,
                    errors = validationEx.Errors
                });
                break;

            case Application.Common.Exceptions.NotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message,
                    statusCode = (int)HttpStatusCode.NotFound
                });
                break;

            case Application.Common.Exceptions.ForbiddenException:
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message,
                    statusCode = (int)HttpStatusCode.Forbidden
                });
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorResponse = env.IsDevelopment()
                    ? new { error = exception.Message, statusCode = (int)HttpStatusCode.InternalServerError, detail = exception.ToString() }
                    : (object)new { error = "An unexpected error occurred", statusCode = (int)HttpStatusCode.InternalServerError };
                await context.Response.WriteAsJsonAsync(errorResponse);
                break;
        }
    }
}
