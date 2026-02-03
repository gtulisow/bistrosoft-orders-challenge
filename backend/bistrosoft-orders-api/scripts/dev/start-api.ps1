# PowerShell script para iniciar la API con configuración de desarrollo
# Uso: .\start-api.ps1

Write-Host "🚀 Iniciando Bistrosoft Orders API" -ForegroundColor Blue
Write-Host "===================================" -ForegroundColor Blue
Write-Host ""

# Navegar al directorio raíz del proyecto
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Join-Path $scriptPath "..\..\"
Set-Location $projectRoot

# Configurar variables de entorno
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:SEED_ADMIN_PASSWORD = "Admin123!"

Write-Host "📋 Configuración:" -ForegroundColor Cyan
Write-Host "   Environment: $env:ASPNETCORE_ENVIRONMENT"
Write-Host "   Admin Password: [CONFIGURADO]"
Write-Host "   Database: InMemory (según appsettings.Development.json)"
Write-Host ""

# Detener cualquier proceso anterior en el puerto 8080
Write-Host "🛑 Deteniendo procesos anteriores en puerto 8080..." -ForegroundColor Yellow
$process = Get-NetTCPConnection -LocalPort 8080 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique
if ($process) {
    Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
    Write-Host "   Proceso detenido" -ForegroundColor Green
}

Write-Host "▶️  Iniciando API..." -ForegroundColor Green
Write-Host ""

dotnet run --project src/Bistrosoft.Orders.Api
