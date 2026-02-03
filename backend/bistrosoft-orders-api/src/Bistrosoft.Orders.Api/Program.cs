using System.Reflection;
using Bistrosoft.Orders.Api.Extensions;
using Bistrosoft.Orders.Api.Middleware;
using Bistrosoft.Orders.Application;
using Bistrosoft.Orders.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog as the logging provider
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

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add CORS policy
builder.Services.AddApiCors(builder.Configuration);

// Add JWT Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure Swagger with XML comments and JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bistrosoft Orders API",
        Description = "REST API for managing orders of an online store using Hexagonal Architecture + CQRS",
        Contact = new OpenApiContact
        {
            Name = "Bistrosoft",
            Email = "dev@bistrosoft.com"
        }
    });

    // Enable XML comments
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add Swagger JWT authentication support
builder.Services.AddSwaggerWithAuth();

// Add Application layer (MediatR)
builder.Services.AddApplication();

// Add Infrastructure layer (DbContext, Repositories, UnitOfWork, Seeder)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Initialize database (migrations + seed)
await DatabaseInitializer.InitializeAsync(app.Services);

// Configure the HTTP request pipeline

// Serilog request logging (debe ir temprano en el pipeline)
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
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
        
        var userId = httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? httpContext.User?.FindFirst("sub")?.Value
                     ?? "anonymous";
        diagnosticContext.Set("UserId", userId);
    };
});

// Middleware de enriquecimiento del LogContext
app.UseMiddleware<LogContextEnricherMiddleware>();

// Global exception handling
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bistrosoft Orders API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// CORS must be called after UseRouting and before UseAuthorization
app.UseCors("ApiCorsPolicy");

// Authentication & Authorization must be in this order
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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
