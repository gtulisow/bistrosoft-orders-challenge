#!/bin/bash
# Script para resetear la base de datos (eliminar y recrear)
# Uso: ./reset-database.sh

echo "🗑️  Reseteando Base de Datos"
echo "============================"
echo ""

cd "$(dirname "$0")/../.." || exit

# Preguntar confirmación
read -p "⚠️  Esto eliminará TODOS los datos. ¿Continuar? (y/N): " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Cancelado"
    exit 0
fi

echo "🛑 Deteniendo API si está corriendo..."
lsof -ti:8080 | xargs kill -9 2>/dev/null || true

echo ""
echo "📁 Eliminando migraciones antiguas..."
rm -rf src/Bistrosoft.Orders.Infrastructure/Persistence/Migrations/*.cs

echo ""
echo "🐳 Reseteando Docker (si está en uso)..."
docker-compose down -v 2>/dev/null || true

echo ""
echo "🆕 Creando nueva migración..."
dotnet ef migrations add InitialCreate \
  --project src/Bistrosoft.Orders.Infrastructure \
  --startup-project src/Bistrosoft.Orders.Api \
  --output-dir Persistence/Migrations

echo ""
echo "✅ Base de datos reseteada"
echo ""
echo "Próximos pasos:"
echo "1. Si usas Docker: ./scripts/dev/start-docker.sh"
echo "2. Iniciar API: ./scripts/dev/start-api.sh"
