# 🛒 Bistrosoft Orders - Full Stack Application

Sistema de gestión de órdenes para una tienda online, implementado con **Arquitectura Hexagonal (Puertos y Adaptadores)** + **CQRS**.

## 📋 Stack Tecnológico

### Backend
- **.NET 8** - API REST
- **Entity Framework Core 8** - ORM
- **SQL Server / Azure SQL Edge** - Base de datos
- **MediatR** - CQRS (Commands/Queries)
- **Swagger/OpenAPI** - Documentación
- **xUnit + Moq** - Testing

### Frontend
- **Vue 3** - Framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Pinia** - State management

### Infrastructure
- **Docker** - Containerización
- **Docker Compose** - Orquestación

## 📁 Estructura del Proyecto

```
bistrosoft-orders-challenge/
├── backend/
│   └── bistrosoft-orders-api/
│       ├── src/
│       │   ├── Bistrosoft.Orders.Api/          # Controllers, Middleware, DI
│       │   ├── Bistrosoft.Orders.Application/  # CQRS (Commands/Queries/Handlers)
│       │   ├── Bistrosoft.Orders.Domain/       # Entities, Value Objects, Enums
│       │   └── Bistrosoft.Orders.Infrastructure/ # EF Core, Repositories, DB
│       ├── tests/
│       │   └── Bistrosoft.Orders.Tests/        # Unit Tests
│       └── Bistrosoft.Orders.sln
├── frontend/
│   └── bistrosoft-orders-web/
│       ├── src/
│       │   ├── api/                            # API clients
│       │   ├── components/                     # Vue components
│       │   ├── views/                          # Pages
│       │   ├── stores/                         # Pinia stores
│       │   └── router/                         # Vue Router
│       └── package.json
├── docs/                                       # Documentación
└── README.md
```

## 🚀 Quick Start

### Prerequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (para frontend)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

## 🗄️ Base de Datos (SQL Server)

### 1. Levantar SQL Server con Docker

```bash
# En la raíz del proyecto backend
docker-compose up -d

# Verificar que esté corriendo
docker ps | grep bistrosoft-sqlserver
```

**Credenciales:**
- Host: `localhost:1433`
- Database: `BistrosoftOrdersDb`
- User: `sa`
- Password: `Admin1234!`

### 2. Detener SQL Server

```bash
docker-compose down

# Para eliminar también los datos
docker-compose down -v
```

---

## 🔧 Backend (.NET 8 API)

### Opción 1: Desde el IDE (Recomendado)

#### Visual Studio Code
```bash
# 1. Abrir carpeta del backend
cd backend/bistrosoft-orders-api
code .

# 2. Presionar F5 para debug
# 3. Seleccionar perfil: "Development (HTTP + Swagger)"
# 4. La API se abre automáticamente en http://localhost:8080/swagger
```

#### Visual Studio 2022
```bash
# 1. Abrir solution
backend/bistrosoft-orders-api/Bistrosoft.Orders.sln

# 2. Presionar F5
# 3. Se abre automáticamente en http://localhost:8080/swagger
```

### Opción 2: Desde Terminal

```bash
# 1. Asegurarse que SQL Server esté corriendo
docker ps | grep bistrosoft-sqlserver

# 2. Navegar a la carpeta del backend
cd backend/bistrosoft-orders-api

# 3. Ejecutar la API
dotnet run --project src/Bistrosoft.Orders.Api

# 4. Abrir en el navegador
# http://localhost:8080/swagger
```

### Compilar y Testear

```bash
cd backend/bistrosoft-orders-api

# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Restaurar dependencias
dotnet restore
```

### Migraciones de Base de Datos

Las migraciones se aplican **automáticamente** al iniciar la API.

Para crear nuevas migraciones:

```bash
cd backend/bistrosoft-orders-api

# Crear migración
dotnet ef migrations add MigrationName \
  --project src/Bistrosoft.Orders.Infrastructure \
  --startup-project src/Bistrosoft.Orders.Api \
  --output-dir Persistence/Migrations

# Aplicar manualmente (opcional)
dotnet ef database update \
  --project src/Bistrosoft.Orders.Infrastructure \
  --startup-project src/Bistrosoft.Orders.Api
```

---

## 🎨 Frontend (Vue 3 + TypeScript)

### Opción 1: Desde el IDE

#### Visual Studio Code
```bash
# 1. Abrir carpeta del frontend
cd frontend/bistrosoft-orders-web
code .

# 2. Instalar dependencias (primera vez)
npm install

# 3. Iniciar dev server
npm run dev

# 4. Abrir en el navegador
# http://localhost:3000
```

### Opción 2: Desde Terminal

```bash
# 1. Navegar a la carpeta del frontend
cd frontend/bistrosoft-orders-web

# 2. Instalar dependencias (primera vez)
npm install

# 3. Configurar variables de entorno
cp .env.example .env

# 4. Iniciar dev server
npm run dev

# 5. Abrir en el navegador
# http://localhost:3000
```

### Comandos del Frontend

```bash
cd frontend/bistrosoft-orders-web

# Dev server
npm run dev

# Build para producción
npm run build

# Preview del build
npm run preview

# Linter
npm run lint
```

### Configuración del API Endpoint

Editar `frontend/bistrosoft-orders-web/.env`:

```env
VITE_API_URL=http://localhost:8080
```

---

## 🐳 Docker - Full Stack

### Backend Dockerizado (Opcional)

```bash
# TODO: Agregar Dockerfile para backend
```

---

## 🧪 Probar la Aplicación

### 1. Usando Swagger (Backend)

```
http://localhost:8080/swagger
```

- Documentación interactiva
- Ejecutar requests directamente
- Ver schemas de DTOs

### 2. Usando el Frontend

```
http://localhost:3000
```

- Interface de usuario completa
- Gestión de clientes
- Creación de órdenes
- Visualización de órdenes

### 3. Usando API Directamente

Ver ejemplos en: `docs/api-examples.http`

```bash
# Crear cliente
curl -X POST http://localhost:8080/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "+1234567890"
  }'
```

---

## 🔄 Flujo de Trabajo Completo

### Desarrollo Local (Full Stack)

```bash
# Terminal 1: Base de datos
docker-compose up -d

# Terminal 2: Backend
cd backend/bistrosoft-orders-api
dotnet run --project src/Bistrosoft.Orders.Api

# Terminal 3: Frontend
cd frontend/bistrosoft-orders-web
npm run dev

# Abrir en el navegador:
# - Frontend: http://localhost:3000
# - Backend Swagger: http://localhost:8080/swagger
```

### Desarrollo Solo Backend

```bash
# Terminal 1: Base de datos
docker-compose up -d

# Terminal 2: Backend
cd backend/bistrosoft-orders-api
dotnet run --project src/Bistrosoft.Orders.Api

# Probar con Swagger:
# http://localhost:8080/swagger
```

---

## 📡 API Endpoints

### Customers
- `POST /api/customers` - Crear cliente
- `GET /api/customers/{id}` - Obtener cliente con órdenes
- `GET /api/customers/{id}/orders` - Listar órdenes del cliente

### Orders
- `POST /api/orders` - Crear orden (valida stock)
- `PUT /api/orders/{id}/status` - Actualizar estado de orden


---

## 🧪 Testing

### Backend Tests

```bash
cd backend/bistrosoft-orders-api

# Ejecutar todos los tests
dotnet test

# Con verbosity
dotnet test --verbosity normal

# Con coverage
dotnet test /p:CollectCoverage=true
```

**Tests incluidos:**
- ✅ Unit tests de Domain entities
- ✅ Unit tests de Application handlers
- ✅ Integration tests de Repositories

---

## 🛠️ Troubleshooting

### Backend no inicia

```bash
# 1. Verificar que SQL Server esté corriendo
docker ps | grep bistrosoft-sqlserver

# 2. Si no está corriendo
docker-compose up -d

# 3. Verificar puerto 8080 libre
lsof -ti:8080
# Si hay proceso, matarlo:
kill -9 $(lsof -ti:8080)
```

### Frontend no conecta con Backend

```bash
# 1. Verificar que backend esté corriendo
curl http://localhost:8080/api/customers

# 2. Verificar variable de entorno
cat frontend/bistrosoft-orders-web/.env
# Debe tener: VITE_API_URL=http://localhost:8080/api

# 3. Verificar CORS en backend (ya configurado)
```

### Base de datos no conecta

```bash
# 1. Verificar que el container esté healthy
docker ps

# 2. Ver logs del container
docker logs bistrosoft-sqlserver

# 3. Recrear container
docker-compose down -v
docker-compose up -d
```

---

## 📚 Documentación Adicional

- **Backend API**: `backend/bistrosoft-orders-api/README.md`
- **Frontend**: `frontend/bistrosoft-orders-web/README.md`

---


## 📄 Licencia

Este proyecto es parte de un desafío técnico para Bistrosoft.
