#!/bin/bash
# Script para iniciar la API con configuración de desarrollo
# Uso: ./start-api.sh

echo "🚀 Iniciando Bistrosoft Orders API"
echo "==================================="
echo ""

cd "$(dirname "$0")/../.." || exit

# Configurar variables de entorno
export ASPNETCORE_ENVIRONMENT=Development
export SEED_ADMIN_PASSWORD="Admin123!"

echo "📋 Configuración:"
echo "   Environment: $ASPNETCORE_ENVIRONMENT"
echo "   Admin Password: [CONFIGURADO]"
echo "   Database: InMemory (según appsettings.Development.json)"
echo ""

# Detener cualquier proceso anterior en el puerto 8080
echo "🛑 Deteniendo procesos anteriores en puerto 8080..."
lsof -ti:8080 | xargs kill -9 2>/dev/null || true

echo "▶️  Iniciando API..."
echo ""

dotnet run --project src/Bistrosoft.Orders.Api

# Nota: Este script se ejecuta en foreground
# Para ejecutar en background, usa: ./start-api.sh &
