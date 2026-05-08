using System.Text.Json;
using DevTrack.Domain.DTOs.Common;
using DevTrack.Domain.Exceptions;
using FluentValidation;

namespace DevTrack.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (ValidationException vex)
        {
            _logger.LogInformation(vex, "Validation failed.");
            await WriteAsync(context, 400, new ApiError
            {
                Code = "VALIDATION_FAILED",
                Message = "One or more validation errors occurred.",
                Details = vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
            });
        }
        catch (ValidationFailedException vex)
        {
            _logger.LogInformation(vex, "Validation failed.");
            await WriteAsync(context, vex.StatusCode, new ApiError
            {
                Code = vex.Code,
                Message = vex.Message,
                Details = vex.Errors
            });
        }
        catch (AppException aex)
        {
            _logger.LogInformation(aex, "Handled application exception: {Code}", aex.Code);
            await WriteAsync(context, aex.StatusCode, new ApiError { Code = aex.Code, Message = aex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            await WriteAsync(context, 500, new ApiError { Code = "INTERNAL_ERROR", Message = "An unexpected error occurred." });
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, ApiError error)
    {
        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ApiResponse<object> { Success = false, Data = null, Error = error };
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }
}
