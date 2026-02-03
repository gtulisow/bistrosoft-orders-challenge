# Implementación de Serilog - Production-Grade Logging

## 📋 Resumen

Sistema de logging estructurado implementado con Serilog para observabilidad en producción, respetando los límites de la arquitectura hexagonal.

---

## 🏗️ Arquitectura de Logging

```
┌─────────────────────────────────────────────────┐
│  HTTP Request                                   │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  Serilog Request Logging Middleware             │
│  - Captura: method, path, status, duration      │
│  - Enriquece: traceId, userId                   │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  LogContextEnricherMiddleware                   │
│  - Pushea a LogContext: TraceId, UserId, IP     │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  GlobalExceptionMiddleware                      │
│  - Warning: DomainExceptions (controladas)      │
│  - Error: Excepciones inesperadas               │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  Controllers                                    │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  Application Handlers (ILogger<T>)              │
│  - Information: Eventos de negocio              │
│  - Warning: Validaciones rechazadas             │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
           Serilog Sinks
        (Console JSON)
```

---

## 📦 Paquetes NuGet Requeridos

### Comandos de Instalación

```bash
cd src/Bistrosoft.Orders.Api

dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Enrichers.Environment
dotnet add package Serilog.Enrichers.Process
dotnet add package Serilog.Enrichers.Thread
dotnet add package Serilog.Settings.Configuration
dotnet add package Serilog.Formatting.Compact
```

**O ejecuta el script:**
```bash
chmod +x SERILOG_SETUP.sh
./SERILOG_SETUP.sh
```

---

## 🔧 Configuración

### 1. Program.cs - Bootstrap de Serilog

```csharp
using Serilog;
using Serilog.Events;

// Configure Serilog ANTES de builder
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId());
```

### 2. Program.cs - Middleware Pipeline

```csharp
// Serilog request logging (temprano en pipeline)
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms | TraceId={TraceId} UserId={UserId}";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null 
        ? LogEventLevel.Error 
        : httpContext.Response.StatusCode > 499 
            ? LogEventLevel.Error 
            : LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
        var userId = httpContext.User?.FindFirst("sub")?.Value ?? "anonymous";
        diagnosticContext.Set("UserId", userId);
    };
});

// LogContext enricher middleware
app.UseMiddleware<LogContextEnricherMiddleware>();
```

### 3. Program.cs - Shutdown Graceful

```csharp
try
{
    Log.Information("Starting Bistrosoft Orders API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
```

---

## 📁 Archivos Nuevos/Modificados

### ✨ NUEVOS

#### `Middleware/LogContextEnricherMiddleware.cs`
```csharp
using System.Diagnostics;
using System.Security.Claims;
using Serilog.Context;

public class LogContextEnricherMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("ClientIp", clientIp))
        {
            await _next(context);
        }
    }
}
```

### 🔄 MODIFICADOS

#### `Middleware/GlobalExceptionMiddleware.cs`
```csharp
// Logging diferenciado por tipo de excepción
if (exception is DomainException)
{
    // Excepciones conocidas → Warning
    _logger.LogWarning(exception,
        "Domain exception occurred | Type={ExceptionType} StatusCode={StatusCode} Method={RequestMethod} Path={RequestPath} TraceId={TraceId}",
        exception.GetType().Name,
        (int)statusCode,
        requestMethod,
        requestPath,
        traceId);
}
else
{
    // Excepciones inesperadas → Error
    _logger.LogError(exception,
        "Unhandled exception occurred | Type={ExceptionType} StatusCode={StatusCode} Method={RequestMethod} Path={RequestPath} TraceId={TraceId}",
        exception.GetType().Name,
        (int)statusCode,
        requestMethod,
        requestPath,
        traceId);
}
```

#### `Application/Commands/.../CreateCustomerCommandHandler.cs`
```csharp
private readonly ILogger<CreateCustomerCommandHandler> _logger;

public async Task<Guid> Handle(...)
{
    // ... lógica de creación ...

    _logger.LogInformation("CustomerCreated | CustomerId={CustomerId} Email={Email}",
        customer.Id,
        request.Email);

    return customer.Id;
}
```

#### `Application/Commands/.../CreateOrderCommandHandler.cs`
```csharp
private readonly ILogger<CreateOrderCommandHandler> _logger;

public async Task<Guid> Handle(...)
{
    // Stock insuficiente
    if (product.StockQuantity < itemRequest.Quantity)
    {
        _logger.LogWarning("OrderCreateRejected_InsufficientStock | ProductId={ProductId} ProductName={ProductName} RequestedQuantity={RequestedQuantity} AvailableStock={AvailableStock}",
            product.Id,
            product.Name,
            itemRequest.Quantity,
            product.StockQuantity);
        throw new ValidationException(...);
    }

    // ... persistir ...

    _logger.LogInformation("OrderCreated | OrderId={OrderId} CustomerId={CustomerId} TotalAmount={TotalAmount} ItemsCount={ItemsCount}",
        order.Id,
        order.CustomerId,
        order.TotalAmount,
        order.Items.Count);

    return order.Id;
}
```

#### `Application/Commands/.../UpdateOrderStatusCommandHandler.cs`
```csharp
private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

public async Task<Unit> Handle(...)
{
    var oldStatusId = order.StatusId;

    try
    {
        order.ChangeStatus(request.NewStatusId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("OrderStatusUpdated | OrderId={OrderId} FromStatusId={FromStatusId} ToStatusId={ToStatusId}",
            order.Id,
            oldStatusId,
            order.StatusId);
    }
    catch (InvalidOrderStatusTransitionException ex)
    {
        _logger.LogWarning("OrderStatusRejected_InvalidTransition | OrderId={OrderId} FromStatusId={FromStatusId} ToStatusId={ToStatusId}",
            request.OrderId,
            oldStatusId,
            request.NewStatusId);
        throw;
    }

    return Unit.Value;
}
```

---

## 📝 Configuración appsettings.json

### Producción (appsettings.json)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning",
        "Microsoft.EntityFrameworkCore.Infrastructure": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithEnvironmentName",
      "WithProcessId",
      "WithThreadId"
    ],
    "Properties": {
      "Application": "Bistrosoft.Orders.Api"
    }
  }
}
```

### Desarrollo (appsettings.Development.json)

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.EntityFrameworkCore.Database.Command": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} | {Properties:j}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

**Diferencias:**
- **Producción:** JSON compacto para parseo automático
- **Desarrollo:** Formato legible para humanos + SQL queries

---

## 📊 Ejemplos de Logs

### Request Log (Información)
```json
{
  "@t": "2026-02-03T10:30:15.123Z",
  "@mt": "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms | TraceId={TraceId} UserId={UserId}",
  "@l": "Information",
  "RequestMethod": "GET",
  "RequestPath": "/api/customers/50057e3f-cbb0-4460-b9f2-c7e511b478e2",
  "StatusCode": 200,
  "Elapsed": 45.2341,
  "TraceId": "00-1234567890abcdef-fedcba0987654321-01",
  "UserId": "8c3f7e2a-1234-5678-9abc-def012345678",
  "Application": "Bistrosoft.Orders.Api",
  "MachineName": "server-01",
  "EnvironmentName": "Production"
}
```

### Business Event Log (CustomerCreated)
```json
{
  "@t": "2026-02-03T10:30:15.100Z",
  "@mt": "CustomerCreated | CustomerId={CustomerId} Email={Email}",
  "@l": "Information",
  "CustomerId": "50057e3f-cbb0-4460-b9f2-c7e511b478e2",
  "Email": "juan@example.com",
  "TraceId": "00-1234567890abcdef-fedcba0987654321-01",
  "UserId": "anonymous",
  "SourceContext": "Bistrosoft.Orders.Application.Commands.Customers.CreateCustomer.CreateCustomerCommandHandler"
}
```

### Business Event Log (OrderCreated)
```json
{
  "@t": "2026-02-03T10:31:20.500Z",
  "@mt": "OrderCreated | OrderId={OrderId} CustomerId={CustomerId} TotalAmount={TotalAmount} ItemsCount={ItemsCount}",
  "@l": "Information",
  "OrderId": "dadbe447-bd2c-4a5a-81fb-98acb56e1330",
  "CustomerId": "50057e3f-cbb0-4460-b9f2-c7e511b478e2",
  "TotalAmount": 22.90,
  "ItemsCount": 1,
  "TraceId": "00-abcdef1234567890-1234567890abcdef-01",
  "UserId": "8c3f7e2a-1234-5678-9abc-def012345678"
}
```

### Warning Log (Stock Insuficiente)
```json
{
  "@t": "2026-02-03T10:32:10.200Z",
  "@mt": "OrderCreateRejected_InsufficientStock | ProductId={ProductId} ProductName={ProductName} RequestedQuantity={RequestedQuantity} AvailableStock={AvailableStock}",
  "@l": "Warning",
  "ProductId": "8ece1b52-e30c-422b-b93a-92ada0753266",
  "ProductName": "Asado de Tira",
  "RequestedQuantity": 10,
  "AvailableStock": 5,
  "TraceId": "00-fedcba0987654321-abcdef1234567890-01"
}
```

### Warning Log (Transición Inválida)
```json
{
  "@t": "2026-02-03T10:33:05.800Z",
  "@mt": "OrderStatusRejected_InvalidTransition | OrderId={OrderId} FromStatusId={FromStatusId} ToStatusId={ToStatusId}",
  "@l": "Warning",
  "OrderId": "dadbe447-bd2c-4a5a-81fb-98acb56e1330",
  "FromStatusId": "00000000-0000-0000-0000-000000000004",
  "ToStatusId": "00000000-0000-0000-0000-000000000002",
  "TraceId": "00-abcd1234-5678-9012-3456-789012abcdef-01"
}
```

### Error Log (Excepción Inesperada)
```json
{
  "@t": "2026-02-03T10:35:00.000Z",
  "@mt": "Unhandled exception occurred | Type={ExceptionType} StatusCode={StatusCode} Method={RequestMethod} Path={RequestPath} TraceId={TraceId}",
  "@l": "Error",
  "@x": "System.NullReferenceException: Object reference not set...\n   at ...",
  "ExceptionType": "NullReferenceException",
  "StatusCode": 500,
  "RequestMethod": "POST",
  "RequestPath": "/api/orders",
  "TraceId": "00-xyz123-456-789-abc-def-01"
}
```

---

## 🎯 Niveles de Log Implementados

| Nivel | Uso | Ejemplos |
|-------|-----|----------|
| **Information** | Eventos de negocio exitosos | `CustomerCreated`, `OrderCreated`, `OrderStatusUpdated` |
| **Warning** | Validaciones rechazadas, reglas de negocio | `InsufficientStock`, `InvalidTransition`, `DuplicateEmail` |
| **Error** | Excepciones inesperadas, crashes | `NullReferenceException`, `DbUpdateException`, etc. |
| **Fatal** | App no puede iniciar | Fallo en startup, configuración crítica faltante |

---

## 🔍 Propiedades Enriquecidas

Todas las requests tienen automáticamente:

| Propiedad | Fuente | Ejemplo |
|-----------|--------|---------|
| `TraceId` | HttpContext.TraceIdentifier | `00-abcd1234...` |
| `UserId` | JWT Claims (NameIdentifier/sub) | `guid` o `anonymous` |
| `ClientIp` | HttpContext.Connection.RemoteIpAddress | `127.0.0.1` |
| `UserAgent` | Request Headers | `Mozilla/5.0 ...` |
| `MachineName` | Environment | `server-01` |
| `EnvironmentName` | Hosting Environment | `Production`, `Development` |
| `ProcessId` | Process.GetCurrentProcess() | `12345` |
| `ThreadId` | Thread.CurrentThread.ManagedThreadId | `8` |
| `Application` | Configuración | `Bistrosoft.Orders.Api` |

---

## 🚫 Datos NO Loggeados (Seguridad)

Para evitar filtrar información sensible:

❌ **NO se loggea:**
- Passwords
- JWT tokens completos
- Request/Response bodies completos
- Direcciones completas
- Números de teléfono
- Detalles de tarjetas de crédito

✅ **SÍ se loggea (seguro):**
- CustomerId, OrderId, ProductId (GUIDs)
- UserId (GUID del usuario autenticado)
- Email (solo cuando es necesario para contexto)
- Montos totales
- Cantidades de items
- Status transitions

---

## 🎓 Patrones Implementados

### 1. Structured Logging
```csharp
// ❌ MAL (string interpolation)
_logger.LogInformation($"Order {orderId} created");

// ✅ BIEN (structured)
_logger.LogInformation("OrderCreated | OrderId={OrderId}", orderId);
```

### 2. Event Names
```csharp
// Formato: EntityAction | Properties
_logger.LogInformation("CustomerCreated | CustomerId={CustomerId}", id);
_logger.LogWarning("OrderCreateRejected_InsufficientStock | ProductId={ProductId}", id);
```

### 3. LogContext Enrichment
```csharp
using (LogContext.PushProperty("OrderId", orderId))
{
    // Todos los logs dentro tienen OrderId automáticamente
    _logger.LogInformation("Processing order...");
    await ProcessItems();
    _logger.LogInformation("Order processed");
}
```

---

## 📈 Observabilidad en Producción

### Queries Útiles (si usas un log aggregator)

**Orders creadas en las últimas 24h:**
```
@mt="OrderCreated" AND @t > now-24h
| stats count() by CustomerId
```

**Stock insuficiente:**
```
@mt="OrderCreateRejected_InsufficientStock"
| stats count() by ProductId
```

**Errores 500:**
```
@l="Error" AND StatusCode=500
| sort @t desc
| limit 100
```

**Performance de requests:**
```
RequestMethod="GET" AND RequestPath="/api/orders"
| stats avg(Elapsed), p95(Elapsed), p99(Elapsed)
```

---

## ✅ Checklist de Validación

### Instalación
- [ ] Todos los paquetes NuGet instalados
- [ ] No hay errores de compilación
- [ ] Application layer solo usa `Microsoft.Extensions.Logging.Abstractions`

### Configuración
- [ ] Serilog configurado en Program.cs
- [ ] Middleware pipeline correcto
- [ ] appsettings.json con configuración Serilog

### Logging
- [ ] Requests loggeadas con duración y status
- [ ] TraceId presente en todos los logs
- [ ] UserId presente (o "anonymous")
- [ ] Eventos de negocio loggeados

### Excepciones
- [ ] DomainExceptions → Warning
- [ ] Unexpected exceptions → Error
- [ ] ProblemDetails incluye traceId
- [ ] No se loggean secrets

---

## 🚀 Deployment

### Variables de Entorno (Producción)

```bash
# Opcional: override de configuración Serilog
SERILOG__MINIMUMLEVEL__DEFAULT=Information
SERILOG__WRITETO__0__NAME=Console

# Para agregar más sinks (ej: Application Insights)
SERILOG__WRITETO__1__NAME=ApplicationInsights
SERILOG__WRITETO__1__ARGS__INSTRUMENTATIONKEY=your-key
```

### Docker Logs

```bash
# Ver logs en tiempo real
docker logs -f bistrosoft-orders-api

# Filtrar por nivel
docker logs bistrosoft-orders-api 2>&1 | grep '"@l":"Error"'

# Parsear JSON
docker logs bistrosoft-orders-api 2>&1 | jq -r 'select(."@l"=="Error")'
```

---

## 🎯 Beneficios Implementados

✅ **Structured Logging** - JSON parseable para agregadores  
✅ **Correlation** - TraceId en todos los logs de una request  
✅ **Context Enrichment** - UserId, IP, UserAgent automáticos  
✅ **Business Events** - Auditoría de eventos importantes  
✅ **Performance Tracking** - Duración de requests  
✅ **Error Categorization** - Warning vs Error según tipo  
✅ **No PII/Secrets** - Solo datos seguros loggeados  
✅ **Hexagonal Compliance** - Application layer solo usa abstracciones  

El sistema está listo para observabilidad en producción. 🎉
