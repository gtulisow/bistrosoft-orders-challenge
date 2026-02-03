# Dockerfile para Bistrosoft Orders API
# Multi-stage build para optimización de tamaño

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files for restore (layer caching optimization)
COPY ["src/Bistrosoft.Orders.Api/Bistrosoft.Orders.Api.csproj", "src/Bistrosoft.Orders.Api/"]
COPY ["src/Bistrosoft.Orders.Application/Bistrosoft.Orders.Application.csproj", "src/Bistrosoft.Orders.Application/"]
COPY ["src/Bistrosoft.Orders.Domain/Bistrosoft.Orders.Domain.csproj", "src/Bistrosoft.Orders.Domain/"]
COPY ["src/Bistrosoft.Orders.Infrastructure/Bistrosoft.Orders.Infrastructure.csproj", "src/Bistrosoft.Orders.Infrastructure/"]

# Restore dependencies (only for API project, it will restore all dependencies)
WORKDIR "/src/src/Bistrosoft.Orders.Api"
RUN dotnet restore "Bistrosoft.Orders.Api.csproj"

# Copy all source code
WORKDIR /src
COPY ["src/", "src/"]

# Build
WORKDIR "/src/src/Bistrosoft.Orders.Api"
RUN dotnet build "Bistrosoft.Orders.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Bistrosoft.Orders.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 appuser \
    && adduser --system --uid 1001 --ingroup appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Environment variables (can be overridden)
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start application
ENTRYPOINT ["dotnet", "Bistrosoft.Orders.Api.dll"]
