# PowerShell script para levantar el stack completo en Docker (API + SQL Server)
# Uso: .\start-full-stack.ps1

Write-Host "🐳 Iniciando Stack Completo en Docker" -ForegroundColor Blue
Write-Host "======================================" -ForegroundColor Blue
Write-Host ""

# Navegar al directorio raíz
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Join-Path $scriptPath "..\..\"
Set-Location $projectRoot

# Verificar si Docker está corriendo
Write-Host "🔍 Verificando Docker..." -ForegroundColor Cyan
try {
    docker info | Out-Null
    Write-Host "✅ Docker está corriendo" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker no está corriendo" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor:" -ForegroundColor Yellow
    Write-Host "1. Abre Docker Desktop"
    Write-Host "2. Espera a que diga 'Docker Desktop is running'"
    Write-Host "3. Ejecuta este script de nuevo"
    exit 1
}
Write-Host ""

# Detener cualquier instancia anterior
Write-Host "🛑 Deteniendo instancias anteriores..." -ForegroundColor Yellow

# Detener contenedores Docker
docker-compose -f docker-compose.full.yml down 2>$null

# Detener procesos locales en puerto 8080
Write-Host "   Liberando puerto 8080..." -ForegroundColor Gray
$process = Get-NetTCPConnection -LocalPort 8080 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique
if ($process) {
    Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
}

Write-Host ""

# Build y levantar servicios
Write-Host "🏗️  Building API Docker image..." -ForegroundColor Cyan
Write-Host "   (Esto puede tardar 1-2 minutos la primera vez)" -ForegroundColor Gray
docker-compose -f docker-compose.full.yml build --no-cache

Write-Host ""
Write-Host "🚀 Levantando servicios..." -ForegroundColor Green
docker-compose -f docker-compose.full.yml up -d

Write-Host ""
Write-Host "⏳ Esperando a que los servicios estén listos..." -ForegroundColor Yellow
Write-Host "   (SQL Server puede tardar 10-15 segundos en iniciar)" -ForegroundColor Gray
Start-Sleep -Seconds 15

Write-Host ""
Write-Host "🔍 Verificando estado de los servicios..." -ForegroundColor Cyan
docker-compose -f docker-compose.full.yml ps

Write-Host ""
Write-Host "==========================================" -ForegroundColor Blue
Write-Host "✅ Stack completo levantado exitosamente!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Servicios disponibles:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   🌐 API Swagger:    http://localhost:8080/swagger"
Write-Host "   🔌 API Base URL:   http://localhost:8080/api"
Write-Host "   🗄️  SQL Server:     localhost:1433"
Write-Host ""
Write-Host "📝 Credenciales SQL Server:" -ForegroundColor Yellow
Write-Host "   Usuario: sa"
Write-Host "   Password: Admin1234!"
Write-Host "   Database: BistrosoftOrdersDb"
Write-Host ""
Write-Host "🔐 Credenciales Admin API:" -ForegroundColor Yellow
Write-Host "   Email: admin@bistrosoft.local"
Write-Host "   Password: [configurado con SEED_ADMIN_PASSWORD]"
Write-Host ""
Write-Host "🔧 Comandos útiles:" -ForegroundColor Cyan
Write-Host "   Ver logs:           docker-compose -f docker-compose.full.yml logs -f"
Write-Host "   Ver logs de API:    docker-compose -f docker-compose.full.yml logs -f api"
Write-Host "   Detener:            docker-compose -f docker-compose.full.yml down"
Write-Host "   Detener y limpiar:  docker-compose -f docker-compose.full.yml down -v"
Write-Host ""
Write-Host "🧪 Probar API:" -ForegroundColor Cyan
Write-Host "   .\scripts\test\test-api.ps1"
