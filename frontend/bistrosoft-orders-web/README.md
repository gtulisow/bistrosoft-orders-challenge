# Bistrosoft Orders - Frontend

Frontend moderno construido con Vue 3 + Vite + TypeScript para el sistema de gestión de pedidos de Bistrosoft.

## 🚀 Características

- ✅ Crear y buscar clientes
- ✅ Crear pedidos con múltiples productos
- ✅ Visualizar lista de pedidos con detalles
- ✅ Actualizar estado de pedidos (Pending → Paid → Shipped → Delivered)
- ✅ Cancelar pedidos desde estado Pending
- ✅ Validación de transiciones de estado
- ✅ Manejo centralizado de errores con ProblemDetails
- ✅ UI moderna y responsiva

## 📋 Requisitos Previos

- Node.js 18+ 
- npm o yarn
- nvm (opcional, pero recomendado)
- Backend de Bistrosoft Orders corriendo (por defecto en `http://localhost:8080`)

## 🛠️ Instalación

1. **Clonar el repositorio** (si aún no lo has hecho)

2. **Navegar a la carpeta del frontend:**
```bash
cd frontend/bistrosoft-orders-web
```

3. **Usar la versión correcta de Node.js** (si tienes nvm instalado):
```bash
nvm use
```
Esto leerá el archivo `.nvmrc` y cambiará automáticamente a Node.js 18.18.0

4. **Instalar dependencias:**
```bash
npm install
```

## ⚙️ Configuración

1. **Crear archivo de variables de entorno:**

Copia el archivo de ejemplo:
```bash
cp .env.example .env
```

2. **Editar el archivo `.env`:**
```env
VITE_API_BASE_URL=http://localhost:8080/api
```

Ajusta la URL según donde esté corriendo tu backend.

## 🏃 Ejecutar la Aplicación

### Modo Desarrollo
```bash
npm run dev
```

La aplicación estará disponible en: `http://localhost:3000`

### Compilar para Producción
```bash
npm run build
```

### Vista Previa de Build de Producción
```bash
npm run preview
```

## 📁 Estructura del Proyecto

```
src/
├── api/                    # Cliente HTTP y llamadas a API
│   ├── http.ts            # Configuración de Axios + interceptores
│   ├── customers.api.ts   # Endpoints de clientes
│   └── orders.api.ts      # Endpoints de pedidos
├── components/            # Componentes reutilizables
│   ├── CustomerForm.vue
│   ├── CustomerLookup.vue
│   ├── CustomerDetails.vue
│   ├── OrderForm.vue
│   ├── OrdersList.vue
│   ├── StatusBadge.vue
│   ├── LoadingOverlay.vue
│   └── ErrorBanner.vue
├── models/                # DTOs y tipos TypeScript
│   └── dtos.ts
├── router/                # Configuración de Vue Router
│   └── index.ts
├── stores/                # Stores de Pinia
│   ├── customer.store.ts
│   └── product.store.ts
├── utils/                 # Utilidades
│   ├── money.ts
│   └── validators.ts
├── views/                 # Páginas principales
│   ├── HomeView.vue
│   ├── CustomersView.vue
│   └── OrdersView.vue
├── App.vue               # Componente raíz
├── main.ts              # Punto de entrada
└── style.css            # Estilos globales
```

## 🔌 Endpoints del Backend Utilizados

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/products` | Listar todos los productos disponibles |
| POST | `/api/customers` | Crear un nuevo cliente |
| GET | `/api/customers/{id}` | Obtener cliente por ID |
| POST | `/api/orders` | Crear un nuevo pedido |
| PUT | `/api/orders/{id}/status` | Actualizar estado del pedido |
| GET | `/api/customers/{id}/orders` | Obtener pedidos de un cliente |

## 🎨 Tecnologías Utilizadas

- **Vue 3** - Framework progresivo de JavaScript
- **Vite** - Build tool ultrarrápido
- **TypeScript** - Superset tipado de JavaScript
- **Vue Router** - Enrutamiento oficial de Vue
- **Pinia** - Store de estado oficial de Vue
- **Axios** - Cliente HTTP basado en promesas
- **CSS puro** - Sin dependencias de UI pesadas

## 📝 Flujo de Estados de Pedidos

```
Pending → Paid → Shipped → Delivered
   ↓
Cancelled (solo desde Pending)
```

## 🧪 Catálogo de Productos

El frontend carga dinámicamente los productos desde el backend mediante `GET /api/products`. Los productos incluyen:

- **id**: Identificador único (GUID)
- **name**: Nombre del producto
- **price**: Precio unitario
- **stockQuantity**: Cantidad disponible en inventario

El store de productos (`stores/product.store.ts`) gestiona el estado de carga y errores, garantizando que el catálogo siempre esté sincronizado con el backend.

## 🐛 Manejo de Errores

La aplicación maneja errores del backend que sigan el estándar ProblemDetails de RFC 7807:

```typescript
{
  type: string
  title: string
  status: number
  detail: string
  errors: Record<string, string[]>  // Errores de validación
}
```

Los mensajes de error se muestran al usuario en banners contextuales.

## 📄 Licencia

Este proyecto es parte del desafío técnico de Bistrosoft Orders.

## 👤 Autor

Desarrollado como parte del Bistrosoft Orders Challenge.
