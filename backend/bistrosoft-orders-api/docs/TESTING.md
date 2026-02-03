# 🧪 Testing Bistrosoft Orders API

Guía completa para probar la API de Bistrosoft Orders.

## 🚀 Iniciar la API

### Opción 1: Desde la terminal

```bash
# Asegurarse que SQL Server esté corriendo
docker ps | grep bistrosoft-sqlserver

# Ejecutar la API
dotnet run --project src/Bistrosoft.Orders.Api
```

### Opción 2: Desde VS Code / Visual Studio

1. Abrir el proyecto en VS Code / Visual Studio
2. Presionar `F5` o ir a `Run > Start Debugging`
3. Seleccionar el perfil deseado:
   - **Development (HTTP + Swagger)** - Abre Swagger automáticamente en HTTP
   - **Development (HTTPS + Swagger)** - Abre Swagger automáticamente en HTTPS
   - **API Only (No Browser)** - No abre navegador
   - **Production Simulation** - Simula entorno de producción

## 📊 Acceder a Swagger UI

Una vez iniciada la API:

```
http://localhost:5000/swagger
```

Swagger UI proporciona:
- 📝 Documentación interactiva de todos los endpoints
- ▶️ Posibilidad de ejecutar requests directamente
- 📋 Ejemplos de request/response
- 🔍 Schemas de DTOs

## 🧪 Métodos de Testing

### 1. Swagger UI (Recomendado para empezar)

1. Abrir `http://localhost:5000/swagger`
2. Expandir cualquier endpoint
3. Click en "Try it out"
4. Editar el JSON del request
5. Click en "Execute"
6. Ver la respuesta

**Ventajas:**
- ✅ No requiere herramientas adicionales
- ✅ Documentación integrada
- ✅ Fácil de usar
- ✅ Ver schemas de DTOs

### 2. REST Client (VS Code Extension)

1. Instalar extensión "REST Client" en VS Code
2. Abrir `docs/api-examples.http`
3. Click en "Send Request" sobre cualquier request
4. Ver respuesta en panel lateral

**Ventajas:**
- ✅ Requests guardados en archivo
- ✅ Variables reutilizables
- ✅ Historial de requests
- ✅ Syntax highlighting

### 3. Bash Script (Automatizado)

```bash
cd docs
./test-api.sh
```

**Ventajas:**
- ✅ Testing automatizado
- ✅ Perfecto para CI/CD
- ✅ Tests repetibles

### 4. cURL (Manual)

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

### 5. Postman

1. Importar colección desde `docs/api-examples.http` (convertir)
2. O crear requests manualmente usando la documentación de Swagger

## 📝 Flujo de Testing Completo

### Paso 1: Obtener IDs de Productos Seeded

```bash
# Los productos se seedean automáticamente al iniciar
# Para ver sus IDs, puedes:

# Opción A: Ver en los logs de la aplicación
# Opción B: Consultar la base de datos
# Opción C: Crear un cliente y ver en Swagger "Try it out" qué productos hay
```

### Paso 2: Crear un Cliente

```http
POST http://localhost:5000/api/customers
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890"
}
```

**Respuesta:**
```json
"customer-guid-here"
```

### Paso 3: Crear una Orden

```http
POST http://localhost:5000/api/orders
Content-Type: application/json

{
  "customerId": "customer-guid-from-step-2",
  "items": [
    {
      "productId": "product-guid-from-seed",
      "quantity": 2
    }
  ]
}
```

**Respuesta:**
```json
"order-guid-here"
```

### Paso 4: Ver el Cliente con sus Órdenes

```http
GET http://localhost:5000/api/customers/customer-guid
```

**Respuesta:**
```json
{
  "id": "guid",
  "name": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "+1234567890",
  "orders": [
    {
      "id": "guid",
      "totalAmount": 2999.98,
      "createdAt": "2026-02-02T10:00:00Z",
      "status": "Pending"
    }
  ]
}
```

### Paso 5: Actualizar Estado de la Orden

```http
PUT http://localhost:5000/api/orders/order-guid/status
Content-Type: application/json

{
  "newStatus": "Paid"
}
```

**Respuesta:** `204 No Content`

### Paso 6: Ver Órdenes con Detalles

```http
GET http://localhost:5000/api/customers/customer-guid/orders
```

## 🎯 Casos de Prueba Importantes

### ✅ Happy Path

1. ✅ Crear cliente
2. ✅ Obtener cliente por ID
3. ✅ Crear orden con productos válidos
4. ✅ Actualizar estados: Pending → Paid → Shipped → Delivered
5. ✅ Ver órdenes del cliente

### ❌ Casos de Error

1. ❌ Email duplicado (400 Bad Request)
2. ❌ Email inválido (400 Bad Request)
3. ❌ Stock insuficiente (400 Bad Request)
4. ❌ Producto no existe (404 Not Found)
5. ❌ Cliente no existe (404 Not Found)
6. ❌ Transición de estado inválida (409 Conflict)
   - Pending → Shipped (debe pasar por Paid)
   - Paid → Cancelled (solo desde Pending)
   - Delivered → cualquier otro estado

### 🔄 Transiciones de Estado Válidas

```
Pending
  ├─→ Paid ✅
  │    └─→ Shipped ✅
  │         └─→ Delivered ✅
  └─→ Cancelled ✅

Todas las demás transiciones: ❌ 409 Conflict
```

## 📋 Checklist de Testing

- [ ] Crear cliente con datos válidos
- [ ] Crear cliente con email duplicado (debe fallar)
- [ ] Crear cliente con email inválido (debe fallar)
- [ ] Obtener cliente existente
- [ ] Obtener cliente inexistente (debe fallar)
- [ ] Crear orden con productos válidos
- [ ] Crear orden con stock insuficiente (debe fallar)
- [ ] Crear orden con producto inexistente (debe fallar)
- [ ] Actualizar estado: Pending → Paid ✅
- [ ] Actualizar estado: Paid → Shipped ✅
- [ ] Actualizar estado: Shipped → Delivered ✅
- [ ] Actualizar estado: Pending → Cancelled ✅
- [ ] Actualizar estado: Pending → Shipped ❌ (debe fallar)
- [ ] Actualizar estado: Paid → Cancelled ❌ (debe fallar)
- [ ] Ver órdenes del cliente
- [ ] Verificar que el stock disminuye al crear orden

## 🛠️ Troubleshooting

### La API no inicia

```bash
# Verificar que SQL Server esté corriendo
docker ps | grep bistrosoft-sqlserver

# Si no está corriendo:
docker-compose up -d

# Verificar logs
docker logs bistrosoft-sqlserver
```

### Error de conexión a la base de datos

```bash
# Verificar connection string en appsettings.Development.json
# Debe ser:
# Server=localhost,1433;Database=BistrosoftOrdersDb;User Id=sa;Password=Admin1234!;TrustServerCertificate=True;
```

### Swagger no muestra los endpoints

```bash
# Verificar que el XML documentation file se esté generando
# Debe estar en: bin/Debug/net8.0/Bistrosoft.Orders.Api.xml

# Rebuild el proyecto
dotnet clean
dotnet build
```

## 📚 Recursos Adicionales

- **Swagger UI**: http://localhost:5000/swagger
- **API Base URL**: http://localhost:5000/api
- **Ejemplos HTTP**: `docs/api-examples.http`
- **Script de testing**: `docs/test-api.sh`

## 🎓 Próximos Pasos

Una vez que hayas probado todos los endpoints:

1. Revisar los logs de la aplicación para ver las queries SQL
2. Inspeccionar la base de datos para ver los datos insertados
3. Probar escenarios de concurrencia (múltiples órdenes simultáneas)
4. Verificar el manejo de errores con payloads inválidos
5. Revisar los response times en Swagger
