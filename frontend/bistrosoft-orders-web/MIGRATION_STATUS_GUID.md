# Migración: OrderStatus de Números a GUIDs

## 📋 Resumen

El backend cambió la estructura de `OrderStatus` de un número simple (0, 1, 2, 3, 4) a un objeto con GUID, nombre y descripción. Se ha actualizado todo el frontend para manejar correctamente esta nueva estructura.

---

## 🔄 Cambio en la Estructura

### ANTES (números)
```json
{
  "id": "order-guid",
  "status": 0,  // Número simple
  "totalAmount": 25.50
}
```

### DESPUÉS (objeto con GUID)
```json
{
  "id": "order-guid",
  "status": {
    "id": "00000000-0000-0000-0000-000000000001",
    "name": "Pending",
    "description": "Order has been created but not yet paid"
  },
  "totalAmount": 25.50
}
```

---

## 📊 GUIDs de Estados

| GUID | Name | Descripción (español) |
|------|------|----------------------|
| `00000000-0000-0000-0000-000000000001` | Pending | Pendiente |
| `00000000-0000-0000-0000-000000000002` | Paid | Pagado |
| `00000000-0000-0000-0000-000000000003` | Shipped | Enviado |
| `00000000-0000-0000-0000-000000000004` | Delivered | Entregado |
| `00000000-0000-0000-0000-000000000005` | Cancelled | Cancelado |

---

## 📁 Archivos Modificados

### 1. **`src/models/dtos.ts`**

#### ANTES
```typescript
export type OrderStatus = 0 | 1 | 2 | 3 | 4

export const OrderStatusEnum = {
  Pending: 0,
  Paid: 1,
  Shipped: 2,
  Delivered: 3,
  Cancelled: 4
} as const

export interface OrderDto {
  status: OrderStatus
}

export interface UpdateOrderStatusRequest {
  status: number
}
```

#### DESPUÉS
```typescript
export interface OrderStatusDto {
  id: string
  name: string
  description: string
}

export const OrderStatusIds = {
  Pending: '00000000-0000-0000-0000-000000000001',
  Paid: '00000000-0000-0000-0000-000000000002',
  Shipped: '00000000-0000-0000-0000-000000000003',
  Delivered: '00000000-0000-0000-0000-000000000004',
  Cancelled: '00000000-0000-0000-0000-000000000005'
} as const

export type OrderStatusId = typeof OrderStatusIds[keyof typeof OrderStatusIds]

export interface OrderDto {
  status: OrderStatusDto  // ← Ahora es un objeto
}

export interface UpdateOrderStatusRequest {
  statusId: string  // ← Ahora envía el GUID
}
```

**Cambios clave:**
- ✅ `OrderStatus` (type simple) → `OrderStatusDto` (interface)
- ✅ Agregado `OrderStatusIds` con los GUIDs constantes
- ✅ `UpdateOrderStatusRequest.status` → `statusId`
- ✅ Todas las referencias actualizadas

---

### 2. **`src/utils/orderStatus.ts`**

#### ANTES
```typescript
export function statusToLabel(status: OrderStatus): string {
  switch (status) {
    case 0: return 'Pendiente'
    case 1: return 'Pagado'
    // ...
  }
}

export interface StatusOption {
  label: string
  value: OrderStatus  // número
}

export function canCancel(status: OrderStatus): boolean {
  return status === 0
}
```

#### DESPUÉS
```typescript
import type { OrderStatusDto } from '@/models/dtos'
import { OrderStatusIds } from '@/models/dtos'

export function statusToLabel(status: OrderStatusDto): string {
  switch (status.id) {  // ← Compara GUIDs
    case OrderStatusIds.Pending: return 'Pendiente'
    case OrderStatusIds.Paid: return 'Pagado'
    // ...
    default: return status.name || 'Desconocido'  // ← Fallback al name
  }
}

export interface StatusOption {
  label: string
  statusId: string  // ← Ahora es GUID
}

export function canCancel(status: OrderStatusDto): boolean {
  return status.id === OrderStatusIds.Pending  // ← Compara GUIDs
}
```

**Cambios clave:**
- ✅ Todas las funciones aceptan `OrderStatusDto` en lugar de número
- ✅ Comparaciones usan `status.id` y `OrderStatusIds`
- ✅ `StatusOption.value` → `statusId`
- ✅ Fallback a `status.name` si GUID desconocido

---

### 3. **`src/api/orders.api.ts`**

#### ANTES
```typescript
async updateOrderStatus(orderId: string, status: OrderStatus): Promise<void> {
  await apiClient.put(`/api/orders/${orderId}/status`, { status })
}
```

#### DESPUÉS
```typescript
async updateOrderStatus(orderId: string, statusId: string): Promise<void> {
  await apiClient.put(`/api/orders/${orderId}/status`, { statusId })
}
```

**Request body:**
```json
// ANTES
{ "status": 1 }

// DESPUÉS
{ "statusId": "00000000-0000-0000-0000-000000000002" }
```

---

### 4. **`src/stores/orders.store.ts`**

#### ANTES
```typescript
import { OrderStatusEnum } from '@/models/dtos'

async function changeStatus(customerId: string, orderId: string, newStatus: OrderStatus) {
  await ordersApi.updateOrderStatus(orderId, newStatus)
}

async function cancelOrder(customerId: string, orderId: string) {
  await changeStatus(customerId, orderId, OrderStatusEnum.Cancelled)  // Número 4
}
```

#### DESPUÉS
```typescript
import { OrderStatusIds } from '@/models/dtos'

async function changeStatus(customerId: string, orderId: string, newStatusId: string) {
  await ordersApi.updateOrderStatus(orderId, newStatusId)
}

async function cancelOrder(customerId: string, orderId: string) {
  await changeStatus(customerId, orderId, OrderStatusIds.Cancelled)  // GUID
}
```

---

### 5. **`src/components/OrdersList.vue`**

#### Script ANTES
```typescript
async function handleStatusChange(orderId: string, newStatus: number) {
  if (!newStatus && newStatus !== 0) return
  
  const statusOption = getNextStatusOptions(order.status)
    .find(opt => opt.value === newStatus)
  
  await ordersStore.changeStatus(props.customerId, orderId, newStatus as OrderStatus)
}
```

#### Script DESPUÉS
```typescript
async function handleStatusChange(orderId: string, newStatusId: string) {
  if (!newStatusId) return
  
  const order = ordersStore.orders.find(o => o.id === orderId)
  if (!order) return
  
  const statusOption = getNextStatusOptions(order.status)
    .find(opt => opt.statusId === newStatusId)  // ← statusId
  
  await ordersStore.changeStatus(props.customerId, orderId, newStatusId)
}
```

#### Template ANTES
```vue
<select @change="(e) => handleStatusChange(order.id, Number((e.target as HTMLSelectElement).value))">
  <option v-for="option in getNextStatusOptions(order.status)" 
          :key="option.value" 
          :value="option.value">
    {{ option.label }}
  </option>
</select>
```

#### Template DESPUÉS
```vue
<select @change="(e) => handleStatusChange(order.id, (e.target as HTMLSelectElement).value)">
  <option v-for="option in getNextStatusOptions(order.status)" 
          :key="option.statusId" 
          :value="option.statusId">
    {{ option.label }}
  </option>
</select>
```

**Cambios:**
- ✅ No más `Number()` conversion
- ✅ `option.value` → `option.statusId`
- ✅ Pasa string GUID directamente

---

### 6. **`src/components/StatusBadge.vue`**

#### ANTES
```typescript
import type { OrderStatus } from '@/models/dtos'

const props = defineProps<{
  status: OrderStatus  // número
}>()
```

#### DESPUÉS
```typescript
import type { OrderStatusDto } from '@/models/dtos'

const props = defineProps<{
  status: OrderStatusDto  // objeto
}>()
```

---

## 🔄 Flujo de Actualización de Estado

### Request al Backend

```typescript
// 1. Usuario selecciona "Marcar como Pagado"
handleStatusChange(orderId, "00000000-0000-0000-0000-000000000002")

// 2. Store llama API
ordersStore.changeStatus(customerId, orderId, "00000000-0000-0000-0000-000000000002")

// 3. API hace PUT
PUT /api/orders/{orderId}/status
Body: {
  "statusId": "00000000-0000-0000-0000-000000000002"
}

// 4. Backend responde 204 No Content

// 5. Frontend recarga pedidos
GET /api/customers/{customerId}/orders

// 6. Response con nuevo status
[
  {
    "id": "order-guid",
    "status": {
      "id": "00000000-0000-0000-0000-000000000002",
      "name": "Paid",
      "description": "Payment has been received and confirmed"
    },
    ...
  }
]
```

---

## 🎨 Renderizado de Status

### Badges
```typescript
// Helper convierte OrderStatusDto a label
statusToLabel(order.status) 
// → "Pagado" (español)

// Helper convierte OrderStatusDto a clase CSS
statusBadgeClass(order.status)
// → "badge-paid"

// Resultado en UI:
// <span class="badge badge-paid">Pagado</span>
```

### Dropdown Options
```typescript
// Para un pedido con status Pending
getNextStatusOptions(order.status)
// → [{ label: "Marcar como Pagado", statusId: "00000000-0000-0000-0000-000000000002" }]

// Renderiza:
// <option value="00000000-0000-0000-0000-000000000002">
//   Marcar como Pagado
// </option>
```

---

## ✅ Compatibilidad

### Backend Response
El frontend ahora espera:
```json
{
  "status": {
    "id": "guid",
    "name": "string",
    "description": "string"
  }
}
```

### Backend Request
El frontend envía:
```json
{
  "statusId": "00000000-0000-0000-0000-000000000002"
}
```

---

## 🧪 Testing

### Verificar Cambios

1. **Ver Pedidos:**
   ```
   ✅ Badges muestran "Pendiente", "Pagado", etc. (no números)
   ✅ Colores correctos según estado
   ```

2. **Cambiar Estado:**
   ```
   ✅ Dropdown muestra opciones correctas
   ✅ Confirmar cambio → Request con statusId (GUID)
   ✅ Pedido se actualiza correctamente
   ✅ Banner de éxito aparece
   ```

3. **Cancelar Pedido:**
   ```
   ✅ Solo habilitado para Pending
   ✅ Envía GUID de Cancelled
   ✅ Estado cambia a "Cancelado"
   ```

4. **Console DevTools:**
   ```javascript
   // Ver request
   PUT /api/orders/xxx/status
   Body: { "statusId": "00000000-0000-0000-0000-000000000002" }
   
   // Ver response
   GET /api/customers/xxx/orders
   Response: [{ status: { id: "...", name: "Paid", ... } }]
   ```

---

## 📊 Comparación

| Aspecto | ANTES (números) | DESPUÉS (GUIDs) |
|---------|-----------------|-----------------|
| **DTO Type** | `type OrderStatus = 0\|1\|2\|3\|4` | `interface OrderStatusDto { id, name, description }` |
| **Status en JSON** | `"status": 0` | `"status": { "id": "guid", ... }` |
| **Request Body** | `{ "status": 1 }` | `{ "statusId": "guid" }` |
| **Helper Input** | `statusToLabel(0)` | `statusToLabel({ id: "guid", ... })` |
| **Comparación** | `status === 0` | `status.id === OrderStatusIds.Pending` |
| **Dropdown Value** | `:value="1"` | `:value="guid-string"` |
| **Type Safety** | Números mágicos | GUIDs constantes |

---

## 🎯 Beneficios del Cambio

### 1. **Mayor Semántica**
```typescript
// ANTES: ¿Qué es 0?
if (status === 0) { ... }

// DESPUÉS: Claro y explícito
if (status.id === OrderStatusIds.Pending) { ... }
```

### 2. **Fallback Inteligente**
```typescript
// Si el backend agrega nuevo estado con GUID desconocido
statusToLabel(unknownStatus)
// → Retorna unknownStatus.name (en lugar de "Desconocido")
```

### 3. **Descripciones Ricas**
```typescript
// Ahora tenemos descripción del backend
status.description
// → "Order has been created but not yet paid"
```

### 4. **Menos Errores**
```typescript
// ANTES: Fácil confundir números
changeStatus(1)  // ¿Paid o algo más?

// DESPUÉS: Imposible confundir
changeStatus(OrderStatusIds.Paid)  // Explícito
```

---

## 🚀 Migración Completa

✅ DTOs actualizados con `OrderStatusDto`  
✅ Helpers trabajan con objetos de status  
✅ API envía `statusId` (GUID)  
✅ Store usa `OrderStatusIds` constantes  
✅ Componentes renderean correctamente  
✅ Type safety preservado  
✅ Backward compatible con fallbacks  
✅ Sin números mágicos  

El sistema está completamente migrado y listo para usar con la nueva estructura de status del backend. 🎉
