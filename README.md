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
# 4. La API se abre automáticamente en http://localhost:5000/swagger
```

#### Visual Studio 2022
```bash
# 1. Abrir solution
backend/bistrosoft-orders-api/Bistrosoft.Orders.sln

# 2. Presionar F5
# 3. Se abre automáticamente en http://localhost:5000/swagger
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
# http://localhost:5000/swagger
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
# http://localhost:5173
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
# http://localhost:5173
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
VITE_API_URL=http://localhost:5000/api
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
http://localhost:5000/swagger
```

- Documentación interactiva
- Ejecutar requests directamente
- Ver schemas de DTOs

### 2. Usando el Frontend

```
http://localhost:5173
```

- Interface de usuario completa
- Gestión de clientes
- Creación de órdenes
- Visualización de órdenes

### 3. Usando API Directamente

Ver ejemplos en: `docs/api-examples.http`

```bash
# Crear cliente
curl -X POST http://localhost:5000/api/customers \
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
# - Frontend: http://localhost:5173
# - Backend Swagger: http://localhost:5000/swagger
```

### Desarrollo Solo Backend

```bash
# Terminal 1: Base de datos
docker-compose up -d

# Terminal 2: Backend
cd backend/bistrosoft-orders-api
dotnet run --project src/Bistrosoft.Orders.Api

# Probar con Swagger:
# http://localhost:5000/swagger
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

### Productos
Los productos se seedean automáticamente:
- Laptop Dell XPS 15 ($1,499.99)
- iPhone 15 Pro ($999.99)
- Sony WH-1000XM5 Headphones ($349.99)
- Samsung 4K Monitor 27" ($399.99)
- Logitech MX Master 3 Mouse ($99.99)

---

## 🏗️ Arquitectura

### Backend - Hexagonal Architecture

```
API Layer (Adapters - Entrada)
  ↓ MediatR
Application Layer (Casos de Uso - Puertos)
  ↓ Interfaces
Infrastructure Layer (Adapters - Salida)
  ↓
Domain Layer (Núcleo - Sin dependencias)
```

**Principios:**
- ✅ Domain independiente de frameworks
- ✅ Application define puertos (interfaces)
- ✅ Infrastructure implementa adaptadores
- ✅ API layer delgado (solo routing)

### Frontend - Component Architecture

```
Views (Pages)
  ↓
Stores (Pinia - State Management)
  ↓
API Clients (HTTP)
  ↓
Backend API
```

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

# 3. Verificar puerto 5000 libre
lsof -ti:5000
# Si hay proceso, matarlo:
kill -9 $(lsof -ti:5000)
```

### Frontend no conecta con Backend

```bash
# 1. Verificar que backend esté corriendo
curl http://localhost:5000/api/customers

# 2. Verificar variable de entorno
cat frontend/bistrosoft-orders-web/.env
# Debe tener: VITE_API_URL=http://localhost:5000/api

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
- **Testing Guide**: `docs/TESTING.md`
- **API Examples**: `docs/api-examples.http`

---

## 🔐 Seguridad

⚠️ **Nota**: Esta aplicación NO tiene autenticación implementada. Todos los endpoints son públicos.

Para agregar JWT Authentication, ver: `frontend/bistrosoft-orders-web/JWT_AUTH_IMPLEMENTATION.md`

---

## 🎯 Features Implementadas

### Backend
- ✅ CRUD de Customers
- ✅ Creación de Orders con validación de stock
- ✅ Actualización de estado de Orders (con validación de transiciones)
- ✅ Listado de Orders por Customer
- ✅ Seed de productos iniciales
- ✅ Global exception handling (ProblemDetails)
- ✅ Swagger documentation
- ✅ EF Core Migrations
- ✅ Unit Tests

### Frontend
- ✅ Gestión de Customers
- ✅ Creación de Orders
- ✅ Listado de Orders
- ✅ State management (Pinia)
- ✅ Responsive design
- ✅ Error handling
- ✅ Loading states

---

## 👨‍💻 Desarrollo

### Estructura de Branches (Opcional)
```bash
main           # Producción
develop        # Desarrollo
feature/*      # Features
bugfix/*       # Bug fixes
```

### Commits Convencionales
```bash
feat: nueva funcionalidad
fix: corrección de bug
docs: documentación
refactor: refactorización
test: tests
```

---

## 📄 Licencia

Este proyecto es parte de un desafío técnico para Bistrosoft.

---

## 🤝 Contacto

Para preguntas sobre el proyecto, contactar al equipo de desarrollo de Bistrosoft.

---

## 🎓 Próximos Pasos / Mejoras Futuras

- [ ] Implementar JWT Authentication
- [ ] Agregar paginación en listados
- [ ] Implementar búsqueda y filtros
- [ ] Agregar más tests (E2E)
- [ ] Dockerizar backend
- [ ] CI/CD pipeline
- [ ] Monitoreo y logging
- [ ] Rate limiting
- [ ] Cache (Redis)
