using System.Net;
using System.Text.Json;
using Bistrosoft.Orders.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bistrosoft.Orders.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment environment)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation Error"),
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            // 409 Conflict is more semantically correct for business rule violations
            // where the state transition is invalid due to current state
            InvalidOrderStatusTransitionException => (HttpStatusCode.Conflict, "Invalid State Transition"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        // SECURITY: In production, don't expose detailed error messages for 500 errors
        var detail = GetSafeErrorDetail(exception, statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{(int)statusCode}"
        };

        // Add trace ID for debugging
        if (context.TraceIdentifier is not null)
        {
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, options);
        return context.Response.WriteAsync(json);
    }

    private string GetSafeErrorDetail(Exception exception, HttpStatusCode statusCode)
    {
        // For known domain exceptions, always show the message (they are safe)
        if (exception is DomainException)
        {
            return exception.Message;
        }

        // For unexpected exceptions (500), hide details in production
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            if (_environment.IsProduction())
            {
                return "An unexpected error occurred. Please contact support with the trace ID.";
            }

            // In development, show full details for debugging
            return exception.Message;
        }

        // For other exceptions, show message
        return exception.Message;
    }
}
