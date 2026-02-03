using System.Diagnostics;
using System.Security.Claims;
using Serilog.Context;

namespace Bistrosoft.Orders.Api.Middleware;

/// <summary>
/// Middleware que enriquece el LogContext de Serilog con información contextual de la request
/// </summary>
public class LogContextEnricherMiddleware
{
    private readonly RequestDelegate _next;

    public LogContextEnricherMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Enriquecer con TraceId
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        
        // Enriquecer con UserId si está autenticado
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                     ?? context.User?.FindFirst("sub")?.Value
                     ?? "anonymous";
        
        // Enriquecer con información de la request
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = context.Request.Headers.UserAgent.ToString();

        // Pushear al LogContext (disponible para todos los logs en esta request)
        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("ClientIp", clientIp))
        using (LogContext.PushProperty("UserAgent", userAgent))
        {
            await _next(context);
        }
    }
}
