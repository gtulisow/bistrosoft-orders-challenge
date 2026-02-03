using System.Reflection;
using Bistrosoft.Orders.Api.Extensions;
using Bistrosoft.Orders.Application;
using Bistrosoft.Orders.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();
