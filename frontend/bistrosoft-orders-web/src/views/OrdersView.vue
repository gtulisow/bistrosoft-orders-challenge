<template>
  <div class="container">
    <h1>Gestión de Pedidos</h1>

    <ErrorBanner 
      v-if="error" 
      :message="error"
      @close="error = null"
    />

    <ErrorBanner 
      v-if="productStore.error" 
      :message="productStore.error"
      @close="productStore.error = null"
    />

    <div class="card">
      <h2>Crear Nuevo Pedido</h2>
      <div v-if="productStore.isLoading" class="loading-inline">
        <div class="spinner-sm"></div>
        <span>Cargando productos...</span>
      </div>
      <OrderForm v-else @order-created="handleOrderCreated" />
    </div>

    <div v-if="customerStore.currentCustomer" class="card">
      <div class="card-header">
        <h2>Pedidos de {{ customerStore.currentCustomer.name }}</h2>
        <button 
          @click="refreshOrders" 
          class="btn btn-secondary btn-sm" 
          :disabled="ordersStore.isLoadingOrders"
        >
          <span v-if="ordersStore.isLoadingOrders">⏳ Actualizando...</span>
          <span v-else>🔄 Actualizar</span>
        </button>
      </div>
      
      <OrdersList 
        :customer-id="customerStore.currentCustomer.id"
        @status-updated="handleStatusUpdated"
      />
    </div>

    <div v-else class="card info-message">
      <p>💡 Selecciona un cliente desde la página de Clientes o ingresa un Customer ID al crear un pedido para ver su lista de pedidos.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useCustomerStore } from '@/stores/customer.store'
import { useProductStore } from '@/stores/product.store'
import { useOrdersStore } from '@/stores/orders.store'
import OrderForm from '@/components/OrderForm.vue'
import OrdersList from '@/components/OrdersList.vue'
import LoadingOverlay from '@/components/LoadingOverlay.vue'
import ErrorBanner from '@/components/ErrorBanner.vue'

const customerStore = useCustomerStore()
const productStore = useProductStore()
const ordersStore = useOrdersStore()
const loading = ref(false)
const error = ref<string | null>(null)

// Cargar productos cuando se monta el componente
onMounted(async () => {
  if (productStore.products.length === 0) {
    await productStore.loadProducts()
  }
})

async function handleOrderCreated(customerId: string) {
  error.value = null
  
  try {
    if (!customerStore.currentCustomer || customerStore.currentCustomer.id !== customerId) {
      await customerStore.fetchCustomer(customerId)
    }
    // Recargar pedidos usando el ordersStore
    await ordersStore.fetchOrders(customerId)
  } catch (err: any) {
    error.value = err.message || 'Error al cargar el cliente después de crear el pedido'
  }
}

async function handleStatusUpdated() {
  // El store ya maneja la recarga automáticamente
}

async function refreshOrders() {
  if (!customerStore.currentCustomer) return
  
  await ordersStore.fetchOrders(customerStore.currentCustomer.id)
}
</script>

<style scoped>
h1 {
  margin-bottom: 2rem;
}

h2 {
  margin: 0;
}

.info-message {
  text-align: center;
  padding: 2rem;
  background-color: var(--gray-50);
  border: 2px dashed var(--gray-300);
}

.info-message p {
  color: var(--gray-600);
  font-size: 1rem;
}
</style>
