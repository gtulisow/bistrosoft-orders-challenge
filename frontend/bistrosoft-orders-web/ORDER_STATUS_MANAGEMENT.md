# Sistema de Gestión de Estados de Pedidos

## 📋 Resumen

Implementación completa de gestión de estados de pedidos con arquitectura limpia, siguiendo el patrón Vue 3 + Pinia + TypeScript.

---

## 🔄 Arquitectura Implementada

```
┌─────────────────────────────────────────────────┐
│  OrdersView.vue                                 │
│  - Botón "Actualizar" funcional                │
│  - Gestión de mensajes de éxito/error          │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  OrdersList.vue (Component)                     │
│  - Tabla de pedidos con estados                │
│  - Dropdown de cambio de estado por fila       │
│  - Botón "Cancelar" condicional                │
│  - Loading state por fila                      │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  useOrdersStore (Pinia)                         │
│  - orders: OrderSummaryDto[]                    │
│  - isLoadingOrders: boolean                     │
│  - ordersError: string | null                   │
│  - successMessage: string | null                │
│  - updating: Record<orderId, boolean>           │
│                                                 │
│  Actions:                                       │
│  - fetchOrders(customerId)                      │
│  - changeStatus(customerId, orderId, newStatus) │
│  - cancelOrder(customerId, orderId)             │
│  - clearMessages()                              │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  ordersApi (API Layer)                          │
│  - getCustomerOrders(customerId)                │
│  - updateOrderStatus(orderId, status)           │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│  Backend API                                    │
│  GET  /api/customers/{id}/orders                │
│  PUT  /api/orders/{id}/status                   │
└─────────────────────────────────────────────────┘
```

---

## 🎯 Estados de Pedidos (Backend Enum)

```csharp
public enum OrderStatus
{
    Pending = 0,     // Pendiente
    Paid = 1,        // Pagado
    Shipped = 2,     // Enviado
    Delivered = 3,   // Entregado
    Cancelled = 4    // Cancelado
}
```

### Transiciones Permitidas

```
Pending (0) → Paid (1)
              ↓
           Shipped (2)
              ↓
          Delivered (3) [FINAL]

Pending (0) → Cancelled (4) [FINAL]
```

---

## 📁 Archivos Creados/Modificados

### ✨ NUEVOS

1. **`src/stores/orders.store.ts`**
   - Store dedicado para gestión de pedidos
   - Estados de carga por fila (`updating: Record<orderId, boolean>`)
   - Mensajes de éxito/error con auto-clear
   - Acciones: `fetchOrders`, `changeStatus`, `cancelOrder`

2. **`src/utils/orderStatus.ts`**
   - Helpers para manejo de estados
   - `statusToLabel(status)`: 0 → "Pendiente", 1 → "Pagado", etc.
   - `statusBadgeClass(status)`: Clases CSS para badges
   - `getNextStatusOptions(status)`: Opciones válidas de transición
   - `canCancel(status)`: true solo para Pending (0)

### 🔄 MODIFICADOS

3. **`src/models/dtos.ts`**
   ```typescript
   // ANTES: enum con strings
   export enum OrderStatus {
     Pending = 'Pending',
     Paid = 'Paid',
     ...
   }

   // DESPUÉS: type numérico alineado con backend
   export type OrderStatus = 0 | 1 | 2 | 3 | 4

   export const OrderStatusEnum = {
     Pending: 0,
     Paid: 1,
     Shipped: 2,
     Delivered: 3,
     Cancelled: 4
   } as const
   ```

4. **`src/api/orders.api.ts`**
   - Renombrado: `getByCustomerId` → `getCustomerOrders`
   - Actualizado: `updateStatus` → `updateOrderStatus` (acepta número directamente)
   - Tipado más fuerte con `OrderStatus` type

5. **`src/components/StatusBadge.vue`**
   - Usa helpers de `orderStatus.ts`
   - Muestra etiquetas en español: "Pendiente", "Pagado", etc.
   - Aplica clases CSS correctas automáticamente

6. **`src/components/OrdersList.vue`**
   - Usa `useOrdersStore` para gestión de estado
   - Dropdown de cambio de estado con opciones dinámicas
   - Botón "Cancelar" siempre visible pero habilitado solo para Pending
   - Loading state por fila durante actualizaciones
   - Banners de éxito/error en la parte superior

7. **`src/views/OrdersView.vue`**
   - Botón "Actualizar" funcional usando `ordersStore.fetchOrders`
   - Muestra "Actualizando..." mientras carga
   - Integración con `useOrdersStore`

8. **`src/stores/customer.store.ts`**
   - Actualizado para usar `ordersApi.getCustomerOrders`

---

## 🎨 UI/UX Implementada

### Tabla de Pedidos

```
┌─────────────────────────────────────────────────────────────────┐
│ ✓ Estado actualizado correctamente                [x]          │ ← Banner éxito
├─────────────────────────────────────────────────────────────────┤
│ ID Pedido │ Fecha    │ Estado     │ Total  │ Acciones          │
├─────────────────────────────────────────────────────────────────┤
│ ▶ 3a4b5... │ 1/2/2026 │ Pendiente  │ $25.50 │ [Cambiar estado ▼]│
│                                              │ [✕ Cancelar]      │
├─────────────────────────────────────────────────────────────────┤
│ ▶ 8c9d1... │ 1/1/2026 │ Pagado     │ $42.00 │ [Cambiar estado ▼]│
│                                              │ [✕ Cancelar] 🚫   │
├─────────────────────────────────────────────────────────────────┤
│ ▶ f2e3a... │ 12/31    │ Entregado  │ $18.75 │ Sin acciones      │
│                                              │ [✕ Cancelar] 🚫   │
└─────────────────────────────────────────────────────────────────┘
```

### Dropdown de Estados (ejemplo para Pending)

```
┌────────────────────────────┐
│ Cambiar estado...         ▼│
├────────────────────────────┤
│ Marcar como Pagado         │ ← Única opción válida
└────────────────────────────┘
```

### Botón Cancelar

- **Pending (0):** `[✕ Cancelar]` ← Habilitado
- **Paid (1):** `[✕ Cancelar] 🚫` ← Deshabilitado
- **Shipped (2):** `[✕ Cancelar] 🚫` ← Deshabilitado
- **Delivered (3):** `[✕ Cancelar] 🚫` ← Deshabilitado
- **Cancelled (4):** `[✕ Cancelar] 🚫` ← Deshabilitado

### Loading States

#### Global (Botón Actualizar)
```
🔄 Actualizar        → Estado normal
⏳ Actualizando...   → Deshabilitado mientras carga
```

#### Por Fila (Cambio de estado)
```
[Cambiar estado ▼]     → Habilitado
[Cambiar estado ▼] 🚫  → Deshabilitado (updating[orderId] = true)

[✕ Cancelar]           → Habilitado (si Pending)
[Actualizando...]      → Mientras cambia estado
```

---

## 🔧 API Calls

### 1. Obtener Pedidos de Cliente
```typescript
GET /api/customers/{customerId}/orders

Response: OrderSummaryDto[]
[
  {
    "id": "guid",
    "totalAmount": 25.50,
    "createdAt": "2026-01-02T10:30:00Z",
    "status": 0  // Pending
  }
]
```

### 2. Actualizar Estado de Pedido
```typescript
PUT /api/orders/{orderId}/status

Request Body:
{
  "status": 1  // Número: 0=Pending, 1=Paid, 2=Shipped, 3=Delivered, 4=Cancelled
}

Response: 204 No Content
```

---

## 🧪 Flujo de Usuario

### Caso 1: Cambiar Estado (Pending → Paid)

1. Usuario selecciona "Marcar como Pagado" del dropdown
2. Aparece confirmación: "¿Confirmar: Marcar como Pagado?"
3. Usuario confirma
4. Frontend:
   - `updating[orderId] = true`
   - Desabilita controles de la fila
   - Llama `PUT /api/orders/{orderId}/status` con `{ status: 1 }`
5. Backend responde 204
6. Frontend:
   - Muestra banner verde: "✓ Estado actualizado correctamente"
   - Recarga pedidos: `GET /api/customers/{customerId}/orders`
   - `updating[orderId] = false`
7. Tabla se actualiza con nuevo estado "Pagado"
8. Después de 3 segundos, banner de éxito desaparece automáticamente

### Caso 2: Cancelar Pedido

1. Usuario ve botón "✕ Cancelar" habilitado (solo en Pending)
2. Click en "✕ Cancelar"
3. Confirmación: "¿Estás seguro de que deseas cancelar este pedido?"
4. Usuario confirma
5. Frontend:
   - Llama `ordersStore.cancelOrder(customerId, orderId)`
   - Internamente usa `changeStatus(customerId, orderId, 4)`
6. Mismo flujo que cambio de estado
7. Pedido aparece con badge "Cancelado" (rojo)

### Caso 3: Actualizar Lista

1. Usuario click en "🔄 Actualizar"
2. Botón cambia a "⏳ Actualizando..." y se deshabilita
3. Llama `ordersStore.fetchOrders(customerId)`
4. Tabla se recarga con datos frescos del backend
5. Botón vuelve a "🔄 Actualizar"

---

## 📊 Estados del Store

```typescript
{
  orders: [
    { id: "abc", status: 0, totalAmount: 25.50, ... },
    { id: "def", status: 1, totalAmount: 42.00, ... }
  ],
  isLoadingOrders: false,
  ordersError: null,
  successMessage: "✓ Estado actualizado correctamente",
  updating: {
    "abc": false,  // No se está actualizando
    "def": true    // Se está actualizando ahora
  }
}
```

---

## 🎓 Helpers de orderStatus.ts

### statusToLabel(status: OrderStatus): string
```typescript
statusToLabel(0)  // → "Pendiente"
statusToLabel(1)  // → "Pagado"
statusToLabel(2)  // → "Enviado"
statusToLabel(3)  // → "Entregado"
statusToLabel(4)  // → "Cancelado"
```

### statusBadgeClass(status: OrderStatus): string
```typescript
statusBadgeClass(0)  // → "badge-pending"
statusBadgeClass(1)  // → "badge-paid"
statusBadgeClass(2)  // → "badge-shipped"
statusBadgeClass(3)  // → "badge-delivered"
statusBadgeClass(4)  // → "badge-cancelled"
```

### getNextStatusOptions(status: OrderStatus): StatusOption[]
```typescript
getNextStatusOptions(0)  
// → [{ label: "Marcar como Pagado", value: 1 }]

getNextStatusOptions(1)  
// → [{ label: "Marcar como Enviado", value: 2 }]

getNextStatusOptions(2)  
// → [{ label: "Marcar como Entregado", value: 3 }]

getNextStatusOptions(3)  
// → []  (estado final)

getNextStatusOptions(4)  
// → []  (estado final)
```

### canCancel(status: OrderStatus): boolean
```typescript
canCancel(0)  // → true  (Pending)
canCancel(1)  // → false (Paid)
canCancel(2)  // → false (Shipped)
canCancel(3)  // → false (Delivered)
canCancel(4)  // → false (Cancelled)
```

---

## ✅ Validaciones Implementadas

### Frontend
1. ✅ Dropdown solo muestra estados válidos según estado actual
2. ✅ Botón cancelar solo habilitado para Pending (0)
3. ✅ Confirmación antes de cambiar estado
4. ✅ Confirmación antes de cancelar
5. ✅ Controles deshabilitados durante actualización
6. ✅ No permite múltiples actualizaciones simultáneas del mismo pedido

### Backend (manejado por el backend)
1. ✅ Valida transiciones de estado
2. ✅ Valida que pedido existe
3. ✅ Retorna ProblemDetails en errores
4. ✅ Gestiona stock (no tocado por frontend)

---

## 🚀 Beneficios de la Arquitectura

### 1. Separación de Responsabilidades
- **API Layer**: Solo HTTP calls
- **Store**: Solo gestión de estado
- **Components**: Solo UI/UX
- **Utils**: Solo lógica de negocio reutilizable

### 2. Type Safety Completo
- DTOs tipados con números (alineados con backend)
- OrderStatus como type literal: `0 | 1 | 2 | 3 | 4`
- IntelliSense funcional en todo el código

### 3. Estados Explícitos
- `isLoadingOrders`: Loading global
- `updating[orderId]`: Loading por fila
- `successMessage`: Feedback de éxito
- `ordersError`: Feedback de error

### 4. UX Profesional
- Confirmaciones antes de acciones destructivas
- Loading states claros
- Mensajes de éxito con auto-clear (3s)
- Errores con botón para cerrar
- Controles deshabilitados durante operaciones

### 5. Mantenibilidad
- Helpers centralizados en `orderStatus.ts`
- Store único para pedidos
- Fácil agregar nuevos estados
- Fácil testear lógica de negocio

---

## 🧪 Testing Manual

### Test 1: Cambio de Estado (Happy Path)
1. ✅ Ir a página de Pedidos con cliente seleccionado
2. ✅ Ver pedido con estado "Pendiente"
3. ✅ Dropdown muestra "Marcar como Pagado"
4. ✅ Seleccionar "Marcar como Pagado"
5. ✅ Confirmar
6. ✅ Ver banner verde "✓ Estado actualizado correctamente"
7. ✅ Pedido cambia a "Pagado"
8. ✅ Banner desaparece después de 3 segundos

### Test 2: Cancelar Pedido
1. ✅ Ver pedido "Pendiente"
2. ✅ Botón "✕ Cancelar" habilitado
3. ✅ Click en "✕ Cancelar"
4. ✅ Confirmar cancelación
5. ✅ Pedido cambia a "Cancelado"
6. ✅ Botón "✕ Cancelar" ahora deshabilitado

### Test 3: Botón Actualizar
1. ✅ Click en "🔄 Actualizar"
2. ✅ Botón cambia a "⏳ Actualizando..."
3. ✅ Se deshabilita
4. ✅ Tabla se recarga
5. ✅ Botón vuelve a "🔄 Actualizar"

### Test 4: Estados Finales
1. ✅ Pedido "Entregado" no muestra dropdown
2. ✅ Muestra "Sin acciones"
3. ✅ Botón cancelar deshabilitado
4. ✅ Pedido "Cancelado" igual comportamiento

### Test 5: Error Handling
1. ✅ Backend retorna error 400
2. ✅ Banner rojo con mensaje de error
3. ✅ Botón para cerrar banner
4. ✅ Controles se rehabilitan
5. ✅ Usuario puede reintentar

---

## 📝 Próximas Mejoras

### 1. Optimistic Updates
```typescript
// Actualizar UI inmediatamente, revertir si falla
const oldStatus = order.status
order.status = newStatus  // Optimistic
try {
  await updateOrderStatus(orderId, newStatus)
} catch {
  order.status = oldStatus  // Revert
}
```

### 2. Bulk Actions
```typescript
// Seleccionar múltiples pedidos y cambiar estado en batch
const selectedOrders = ref<string[]>([])
async function bulkChangeStatus(newStatus: OrderStatus) {
  // ...
}
```

### 3. Historial de Cambios
```typescript
interface StatusHistory {
  oldStatus: OrderStatus
  newStatus: OrderStatus
  changedAt: string
  changedBy: string
}
```

### 4. Validación de Permisos
```typescript
// Verificar que usuario tiene permiso para cambiar estado
function canChangeStatus(userRole: string, status: OrderStatus): boolean {
  if (userRole === 'admin') return true
  if (userRole === 'warehouse' && status === OrderStatusEnum.Shipped) return true
  // ...
}
```

---

## ✨ Conclusión

Sistema completo de gestión de estados de pedidos implementado con arquitectura limpia, siguiendo mejores prácticas de Vue 3 + Pinia + TypeScript. Código production-ready, mantenible y escalable.
