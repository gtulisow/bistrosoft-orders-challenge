# PowerShell script para resetear la base de datos (eliminar y recrear)
# Uso: .\reset-database.ps1

Write-Host "🗑️  Reseteando Base de Datos" -ForegroundColor Red
Write-Host "============================" -ForegroundColor Red
Write-Host ""

# Navegar al directorio raíz
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Join-Path $scriptPath "..\..\"
Set-Location $projectRoot

# Preguntar confirmación
$confirmation = Read-Host "⚠️  Esto eliminará TODOS los datos. ¿Continuar? (y/N)"

if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Host "❌ Cancelado" -ForegroundColor Red
    exit 0
}

Write-Host ""
Write-Host "🛑 Deteniendo API si está corriendo..." -ForegroundColor Yellow
$process = Get-NetTCPConnection -LocalPort 8080 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique
if ($process) {
    Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
}

Write-Host ""
Write-Host "📁 Eliminando migraciones antiguas..." -ForegroundColor Cyan
Remove-Item "src\Bistrosoft.Orders.Infrastructure\Persistence\Migrations\*.cs" -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "🐳 Reseteando Docker (si está en uso)..." -ForegroundColor Cyan
docker-compose down -v 2>$null

Write-Host ""
Write-Host "🆕 Creando nueva migración..." -ForegroundColor Cyan
dotnet ef migrations add InitialCreate `
  --project src\Bistrosoft.Orders.Infrastructure `
  --startup-project src\Bistrosoft.Orders.Api `
  --output-dir Persistence\Migrations

Write-Host ""
Write-Host "✅ Base de datos reseteada" -ForegroundColor Green
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Yellow
Write-Host "1. Si usas Docker: .\scripts\dev\start-docker.ps1"
Write-Host "2. Iniciar API: .\scripts\dev\start-api.ps1"
