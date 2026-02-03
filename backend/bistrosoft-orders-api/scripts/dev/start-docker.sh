#!/bin/bash
# Script para iniciar SQL Server con Docker
# Uso: ./start-docker.sh

echo "🐳 Iniciando SQL Server con Docker"
echo "==================================="
echo ""

cd "$(dirname "$0")/../.." || exit

# Verificar si Docker está corriendo
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker no está corriendo"
    echo "Por favor inicia Docker Desktop primero"
    exit 1
fi

echo "✅ Docker está corriendo"
echo ""

# Iniciar servicios
echo "📦 Iniciando contenedores..."
docker-compose up -d

echo ""
echo "✅ SQL Server iniciado"
echo ""
echo "📋 Información:"
echo "   Host: localhost"
echo "   Port: 1433"
echo "   User: sa"
echo "   Password: Admin1234!"
echo "   Database: BistrosoftOrdersDb (se crea al iniciar la API)"
echo ""
echo "🔍 Ver logs: docker-compose logs -f"
echo "🛑 Detener: docker-compose down"
echo "🗑️  Detener y eliminar datos: docker-compose down -v"
