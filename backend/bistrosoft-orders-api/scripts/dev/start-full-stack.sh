#!/bin/bash
# Script para levantar el stack completo en Docker (API + SQL Server)
# Uso: ./start-full-stack.sh

echo "🐳 Iniciando Stack Completo en Docker"
echo "======================================"
echo ""

cd "$(dirname "$0")/../.." || exit

# Verificar si Docker está corriendo
echo "🔍 Verificando Docker..."
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker no está corriendo"
    echo ""
    echo "Por favor:"
    echo "1. Abre Docker Desktop"
    echo "2. Espera a que diga 'Docker Desktop is running'"
    echo "3. Ejecuta este script de nuevo"
    exit 1
fi
echo "✅ Docker está corriendo"
echo ""

# Detener cualquier instancia anterior
echo "🛑 Deteniendo instancias anteriores..."

# Detener contenedores Docker
docker-compose -f docker-compose.full.yml down 2>/dev/null || true

# Detener procesos locales en puerto 8080
echo "   Liberando puerto 8080..."
lsof -ti:8080 | xargs kill -9 2>/dev/null || true
pkill -f "Bistrosoft.Orders.Api" 2>/dev/null || true

echo ""

# Build y levantar servicios
echo "🏗️  Building API Docker image..."
echo "   (Esto puede tardar 1-2 minutos la primera vez)"
docker-compose -f docker-compose.full.yml build --no-cache

echo ""
echo "🚀 Levantando servicios..."
docker-compose -f docker-compose.full.yml up -d

echo ""
echo "⏳ Esperando a que SQL Server esté listo..."
echo "   (Puede tardar 20-30 segundos en iniciar completamente)"
sleep 30

echo ""
echo "🔍 Verificando que SQL Server esté healthy..."
RETRIES=0
MAX_RETRIES=12
until docker inspect bistrosoft-sqlserver | grep -q '"Status": "healthy"' || [ $RETRIES -eq $MAX_RETRIES ]; do
  echo "   Intento $((RETRIES+1))/$MAX_RETRIES - Esperando..."
  sleep 5
  RETRIES=$((RETRIES+1))
done

if [ $RETRIES -eq $MAX_RETRIES ]; then
  echo "⚠️  SQL Server no está healthy, pero continuando..."
else
  echo "✅ SQL Server está healthy"
fi

echo ""
echo "🔍 Verificando estado de los servicios..."
docker-compose -f docker-compose.full.yml ps

echo ""
echo "=========================================="
echo "✅ Stack completo levantado exitosamente!"
echo ""
echo "📋 Servicios disponibles:"
echo ""
echo "   🌐 API Swagger:    http://localhost:8080/swagger"
echo "   🔌 API Base URL:   http://localhost:8080/api"
echo "   🗄️  SQL Server:     localhost:1433"
echo ""
echo "📝 Credenciales SQL Server:"
echo "   Usuario: sa"
echo "   Password: Admin1234!"
echo "   Database: BistrosoftOrdersDb"
echo ""
echo "🔐 Credenciales Admin API:"
echo "   Email: admin@bistrosoft.local"
echo "   Password: [configurado con SEED_ADMIN_PASSWORD]"
echo ""
echo "🔧 Comandos útiles:"
echo "   Ver logs:           docker-compose -f docker-compose.full.yml logs -f"
echo "   Ver logs de API:    docker-compose -f docker-compose.full.yml logs -f api"
echo "   Detener:            docker-compose -f docker-compose.full.yml down"
echo "   Detener y limpiar:  docker-compose -f docker-compose.full.yml down -v"
echo ""
echo "🧪 Probar API:"
echo "   ./scripts/test/test-api.sh"
