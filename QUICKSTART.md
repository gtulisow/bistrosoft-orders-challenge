# ⚡ Quick Start Guide

Guía rápida para levantar el proyecto Bistrosoft Orders en 5 minutos.

## 🎯 Lo Mínimo para Empezar

### 1️⃣ Levantar Base de Datos (SQL Server)

```bash
# En la raíz del proyecto
docker-compose up -d

# Verificar que esté corriendo
docker ps
# Debe mostrar: bistrosoft-sqlserver (Up)
```

### 2️⃣ Levantar Backend (API .NET 8)

```bash
cd backend/bistrosoft-orders-api

# Primera vez: restaurar paquetes
dotnet restore

# Ejecutar la API
dotnet run --project src/Bistrosoft.Orders.Api
```

**¡Listo!** La API está corriendo en:
- Swagger: `http://localhost:5000/swagger`
- API: `http://localhost:5000/api`

### 3️⃣ Levantar Frontend (Vue.js) - Opcional

```bash
cd frontend/bistrosoft-orders-web

# Primera vez: instalar dependencias
npm install

# Copiar configuración
cp .env.example .env

# Ejecutar dev server
npm run dev
```

**¡Listo!** El frontend está corriendo en:
- App: `http://localhost:5173`

---

## 🎮 Probar la API

### Opción 1: Swagger UI (Recomendado)

Abrir `http://localhost:5000/swagger` y usar la interfaz gráfica.

### Opción 2: cURL

```bash
# 1. Crear un cliente
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "+1234567890"
  }'
# Respuesta: "customer-guid"

# 2. Ver el cliente
curl http://localhost:5000/api/customers/customer-guid
```

---

## 🛑 Detener Todo

```bash
# Detener backend: Ctrl+C en la terminal

# Detener frontend: Ctrl+C en la terminal

# Detener base de datos
docker-compose down
```

---

## 🔧 Comandos Útiles

### Backend
```bash
cd backend/bistrosoft-orders-api

dotnet build                                    # Compilar
dotnet test                                     # Ejecutar tests
dotnet run --project src/Bistrosoft.Orders.Api  # Ejecutar API
```

### Frontend
```bash
cd frontend/bistrosoft-orders-web

npm install     # Instalar dependencias
npm run dev     # Dev server
npm run build   # Build producción
```

### Base de Datos
```bash
docker-compose up -d        # Iniciar
docker-compose down         # Detener
docker-compose down -v      # Detener y eliminar datos
docker ps                   # Ver containers corriendo
docker logs bistrosoft-sqlserver  # Ver logs
```

---

## 🐛 Problemas Comunes

### Puerto 5000 ocupado
```bash
# Encontrar y matar proceso
lsof -ti:5000 | xargs kill -9

# O cambiar puerto en launchSettings.json
```

### Base de datos no conecta
```bash
# Recrear container
docker-compose down -v
docker-compose up -d

# Esperar 30 segundos y reintentar
```

### Frontend no conecta con backend
```bash
# Verificar que backend esté corriendo
curl http://localhost:5000/api/customers

# Verificar .env en frontend
cat frontend/bistrosoft-orders-web/.env
# Debe tener: VITE_API_URL=http://localhost:5000/api
```

---

## 📖 Más Información

Para documentación completa, ver [`README.md`](./README.md)
