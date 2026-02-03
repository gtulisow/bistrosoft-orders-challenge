# Bistrosoft Orders Challenge

REST API para gestión de órdenes de una tienda online, implementada con .NET 8 y Arquitectura Hexagonal (Puertos y Adaptadores).

## 🏗️ Arquitectura

El proyecto sigue los principios de **Arquitectura Hexagonal** con separación clara de responsabilidades:

- **Domain**: Entidades del negocio, value objects, enums y excepciones del dominio (independiente de frameworks)
- **Application**: Casos de uso (Commands/Queries), Handlers con MediatR, DTOs y puertos (interfaces)
- **Infrastructure**: Adaptadores (EF Core, Repositorios, configuración de BD)
- **Api**: Controladores REST, middleware, configuración de Swagger
- **Tests**: Tests unitarios con xUnit + Moq

## 🚀 Tecnologías

- **.NET 8**
- **EF Core 8** con **SQL Server**
- **MediatR** (CQRS pattern)
- **Swagger/OpenAPI**
- **xUnit + Moq** para testing
- **Docker** para SQL Server

## 📋 Prerequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (para SQL Server)
- [EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

## 🐳 Configuración de SQL Server con Docker

Inicia SQL Server usando Docker Compose:

```bash
docker-compose up -d
```

Esto levantará Azure SQL Edge en `localhost:1433` con:
- Usuario: `sa`
- Contraseña: `Admin1234!`
- Base de datos: `BistrosoftOrdersDb` (se crea automáticamente con las migraciones)

**Nota**: Se usa Azure SQL Edge en lugar de SQL Server porque es compatible con arquitecturas ARM (macOS Apple Silicon).

Para detener el contenedor:

```bash
docker-compose down
```

Para detener y eliminar los datos:

```bash
docker-compose down -v
```

## 🗃️ Migraciones de Base de Datos

### Crear una nueva migración

```bash
dotnet ef migrations add MigrationName \
  --project src/Bistrosoft.Orders.Infrastructure \
  --startup-project src/Bistrosoft.Orders.Api \
  --output-dir Persistence/Migrations
```

### Aplicar migraciones

Las migraciones se aplican automáticamente al iniciar la API. También puedes aplicarlas manualmente:

```bash
dotnet ef database update \
  --project src/Bistrosoft.Orders.Infrastructure \
  --startup-project src/Bistrosoft.Orders.Api
```

### Revertir migraciones

```bash
dotnet ef database update PreviousMigrationName \
  --project src/Bistrosoft.Orders.Infrastructure \
  --startup-project src/Bistrosoft.Orders.Api
```

## ▶️ Ejecutar la Aplicación

1. **Asegúrate de que SQL Server esté corriendo** (docker-compose up -d)

2. **Ejecuta la API:**

```bash
dotnet run --project src/Bistrosoft.Orders.Api
```

3. **Abre Swagger en tu navegador:**

```
http://localhost:5000/swagger
```

La API aplicará automáticamente las migraciones pendientes y hará el seed de productos iniciales.

## 🧪 Ejecutar Tests

```bash
dotnet test
```

Los tests usan EF Core InMemory y no requieren SQL Server.

## 📦 Compilar la Solución

```bash
dotnet build
```

## 🏷️ Estructura del Proyecto

```
.
├── src/
│   ├── Bistrosoft.Orders.Api/          # Controllers, Middleware, Program.cs
│   ├── Bistrosoft.Orders.Application/  # Commands, Queries, Handlers, DTOs, Interfaces
│   ├── Bistrosoft.Orders.Domain/       # Entidades, ValueObjects, Enums, Excepciones
│   └── Bistrosoft.Orders.Infrastructure/ # DbContext, Repositories, Migrations, Seed
├── tests/
│   └── Bistrosoft.Orders.Tests/        # Tests unitarios (xUnit + Moq)
├── docs/
├── docker-compose.yml                  # SQL Server container
├── Bistrosoft.Orders.sln
└── README.md
```

## 📡 API Endpoints (Planificados)

- `POST /api/customers` - Crear cliente
- `GET /api/customers/{id}` - Obtener cliente con sus órdenes
- `POST /api/orders` - Crear orden (valida stock)
- `PUT /api/orders/{id}/status` - Actualizar estado de orden
- `GET /api/customers/{id}/orders` - Listar órdenes de un cliente

## 🔧 Configuración

La configuración de la aplicación se encuentra en:
- `appsettings.json` - Configuración general
- `appsettings.Development.json` - Configuración de desarrollo (connection string, CORS)

### Connection String

Modifica el connection string en `appsettings.Development.json` si necesitas usar otra instancia de SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BistrosoftOrdersDb;User Id=sa;Password=Admin1234!;TrustServerCertificate=True;"
  }
}
```

### CORS (Cross-Origin Resource Sharing)

La API está configurada para permitir peticiones desde orígenes específicos. Por defecto en **Development**, el frontend en `http://localhost:3000` está permitido.

#### Configuración por Ambiente

**Development** (`appsettings.Development.json`):
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000"
    ],
    "AllowCredentials": false
  }
}
```

**Production** (`appsettings.json` o variables de entorno):
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ],
    "AllowCredentials": false
  }
}
```

#### Agregar Más Orígenes

Para permitir múltiples orígenes (ej: diferentes puertos de desarrollo):

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:8080",
      "http://localhost:5173"
    ],
    "AllowCredentials": false
  }
}
```

#### Habilitar Credenciales

Si tu frontend necesita enviar cookies o headers de autenticación:

```json
{
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"],
    "AllowCredentials": true
  }
}
```

**Nota de Seguridad**: En producción, **nunca** uses `"AllowedOrigins": ["*"]` con `"AllowCredentials": true`. Siempre especifica los dominios exactos permitidos.

## 📝 Notas de Implementación

- **Domain**: Completamente independiente, sin referencias a EF Core o ASP.NET
- **Value Objects**: Email implementado como value object con validación
- **Invariantes del Dominio**: Validaciones en constructores y métodos del dominio
- **Transiciones de Estado**: Order.Status valida transiciones (Pending → Paid → Shipped → Delivered)
- **Repository Pattern**: Interfaces en Application, implementaciones en Infrastructure
- **CQRS**: Separación de Commands (escritura) y Queries (lectura) con MediatR
- **Migraciones**: EF Core migrations para gestión de esquema de BD

## 👨‍💻 Desarrollo

Para agregar nuevas funcionalidades:

1. Define la entidad en **Domain**
2. Crea Commands/Queries y Handlers en **Application**
3. Implementa repositorio en **Infrastructure**
4. Agrega controller en **Api**
5. Escribe tests en **Tests**

## 📄 Licencia

Este proyecto es parte de un desafío técnico para Bistrosoft.
