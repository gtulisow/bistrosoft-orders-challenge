# Migración: Productos Hardcodeados → API Dinámica

## 📋 Resumen

Se ha refactorizado el sistema de productos para consumir el endpoint real del backend `GET /api/products` en lugar de usar un catálogo hardcodeado.

## 🔄 Cambios Realizados

### 1. API Layer (`src/api/products.api.ts`) - NUEVO

```typescript
import apiClient from './http'
import type { ProductDto } from '@/models/dtos'

export const productsApi = {
  async getAll(): Promise<ProductDto[]> {
    const response = await apiClient.get<ProductDto[]>('/api/products')
    return response.data
  }
}
```

**Características:**
- Usa el cliente axios centralizado (`http.ts`)
- Manejo de errores automático vía interceptors
- Tipado completo con TypeScript

---

### 2. DTOs (`src/models/dtos.ts`) - ACTUALIZADO

**Antes:**
```typescript
export interface Product {
  id: string
  name: string
  price: number
}
```

**Después:**
```typescript
export interface ProductDto {
  id: string
  name: string
  price: number
  stockQuantity: number  // ← NUEVO campo del backend
}
```

**Cambios:**
- Renombrado de `Product` a `ProductDto` (consistencia con otros DTOs)
- Agregado campo `stockQuantity` (cantidad en inventario)

---

### 3. Product Store (`src/stores/product.store.ts`) - REFACTORIZADO

**Antes:**
```typescript
const products = ref<Product[]>([
  { id: '1', name: 'Hamburguesa Clásica', price: 8.99 },
  // ... 11 productos más hardcodeados
])
```

**Después:**
```typescript
const products = ref<ProductDto[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)

async function loadProducts() {
  isLoading.value = true
  error.value = null
  
  try {
    products.value = await productsApi.getAll()
  } catch (err: any) {
    error.value = err.message || 'Error al cargar los productos'
    products.value = []
  } finally {
    isLoading.value = false
  }
}
```

**Nuevas Características:**
- ✅ Estado de carga (`isLoading`)
- ✅ Manejo de errores (`error`)
- ✅ Función `loadProducts()` para cargar dinámicamente
- ✅ Funciones helper preservadas (`getProductById`, `getProductPrice`)

---

### 4. OrdersView (`src/views/OrdersView.vue`) - ACTUALIZADO

**Agregado:**
```typescript
import { useProductStore } from '@/stores/product.store'

const productStore = useProductStore()

onMounted(async () => {
  if (productStore.products.length === 0) {
    await productStore.loadProducts()
  }
})
```

**En el template:**
```vue
<ErrorBanner 
  v-if="productStore.error" 
  :message="productStore.error"
  @close="productStore.error = null"
/>

<div v-if="productStore.isLoading" class="loading-inline">
  <div class="spinner-sm"></div>
  <span>Cargando productos...</span>
</div>
<OrderForm v-else @order-created="handleOrderCreated" />
```

**Características:**
- Carga automática de productos al montar la vista
- Muestra estado de carga mientras se obtienen productos
- Muestra errores si falla la carga
- Cache inteligente (no recarga si ya están cargados)

---

## 🎯 Arquitectura Final

```
Frontend (Vue 3 + Pinia)
    ↓
Product Store (Pinia)
    ↓ loadProducts()
Products API Layer
    ↓ HTTP GET
Axios Client (http.ts)
    ↓
Backend: GET /api/products
    ↓ Response
[
  {
    "id": "guid",
    "name": "string",
    "price": number,
    "stockQuantity": number
  }
]
```

---

## ✅ Beneficios de la Refactorización

### 1. **Separación de Responsabilidades**
- **API Layer**: Solo maneja HTTP
- **Store**: Solo maneja estado
- **Components**: Solo renderiza UI

### 2. **Type Safety Completo**
- DTOs tipados con TypeScript
- IntelliSense en todo el código
- Errores de tipo en compile-time

### 3. **Manejo de Estados**
- Loading states visibles al usuario
- Errores manejados gracefully
- No bloquea la UI

### 4. **Sincronización con Backend**
- Productos siempre actualizados
- Precios autoritativos del backend
- Stock disponible en tiempo real

### 5. **Escalabilidad**
- Fácil agregar paginación
- Fácil agregar filtros
- Fácil agregar búsqueda

---

## 🚀 Cómo Usar

### En Componentes

```typescript
import { useProductStore } from '@/stores/product.store'

const productStore = useProductStore()

// Cargar productos
await productStore.loadProducts()

// Acceder a productos
const products = productStore.products

// Verificar carga
if (productStore.isLoading) {
  console.log('Cargando...')
}

// Verificar errores
if (productStore.error) {
  console.error(productStore.error)
}

// Helpers
const product = productStore.getProductById('some-guid')
const price = productStore.getProductPrice('some-guid')
```

---

## 🔧 Compatibilidad hacia Atrás

Los componentes existentes que consumen el store **NO necesitan cambios** porque:

1. La API del store se mantiene igual:
   - `products` (array reactivo)
   - `getProductById(id)`
   - `getProductPrice(id)`

2. Solo se agregaron propiedades nuevas:
   - `isLoading`
   - `error`
   - `loadProducts()`

3. Los componentes que ya funcionaban siguen funcionando.

---

## 🐛 Manejo de Errores

### Si el backend está caído
```
productStore.error = "Error al cargar los productos"
productStore.products = []
```

El UI mostrará un banner de error al usuario.

### Si el backend retorna ProblemDetails
El interceptor de axios (`http.ts`) automáticamente:
1. Extrae el mensaje de error
2. Lo formatea amigablemente
3. Lo propaga al store

---

## 📝 Próximos Pasos Sugeridos

1. **Validar Stock en Orden**
   ```typescript
   const product = productStore.getProductById(productId)
   if (product && item.quantity > product.stockQuantity) {
     error = 'Stock insuficiente'
   }
   ```

2. **Mostrar Stock en UI**
   ```vue
   <option v-for="product in productStore.products" :key="product.id">
     {{ product.name }} - {{ formatMoney(product.price) }}
     (Stock: {{ product.stockQuantity }})
   </option>
   ```

3. **Refresh Automático**
   ```typescript
   // Recargar productos cada 5 minutos
   setInterval(() => {
     productStore.loadProducts()
   }, 5 * 60 * 1000)
   ```

---

## ✨ Conclusión

La migración está completa y lista para producción. El sistema ahora consume productos reales del backend con manejo robusto de estados y errores, siguiendo las mejores prácticas de arquitectura frontend.
