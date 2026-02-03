# PowerShell script para iniciar SQL Server con Docker
# Uso: .\start-docker.ps1

Write-Host "🐳 Iniciando SQL Server con Docker" -ForegroundColor Blue
Write-Host "===================================" -ForegroundColor Blue
Write-Host ""

# Navegar al directorio raíz del proyecto
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Join-Path $scriptPath "..\..\"
Set-Location $projectRoot

# Verificar si Docker está corriendo
try {
    docker info | Out-Null
    Write-Host "✅ Docker está corriendo" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker no está corriendo" -ForegroundColor Red
    Write-Host "Por favor inicia Docker Desktop primero" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Iniciar servicios
Write-Host "📦 Iniciando contenedores..." -ForegroundColor Cyan
docker-compose up -d

Write-Host ""
Write-Host "✅ SQL Server iniciado" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Información:" -ForegroundColor Cyan
Write-Host "   Host: localhost"
Write-Host "   Port: 1433"
Write-Host "   User: sa"
Write-Host "   Password: Admin1234!"
Write-Host "   Database: BistrosoftOrdersDb (se crea al iniciar la API)"
Write-Host ""
Write-Host "🔍 Ver logs: docker-compose logs -f" -ForegroundColor Yellow
Write-Host "🛑 Detener: docker-compose down" -ForegroundColor Yellow
Write-Host "🗑️  Detener y eliminar datos: docker-compose down -v" -ForegroundColor Yellow
