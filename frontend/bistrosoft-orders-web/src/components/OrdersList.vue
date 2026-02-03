<template>
  <div class="orders-container">
    <!-- Mensajes de éxito/error -->
    <div v-if="ordersStore.successMessage" class="alert alert-success">
      {{ ordersStore.successMessage }}
    </div>

    <div v-if="ordersStore.ordersError" class="alert alert-error">
      {{ ordersStore.ordersError }}
      <button @click="ordersStore.clearMessages()" class="close-btn">✕</button>
    </div>

    <!-- Loading state -->
    <div v-if="ordersStore.isLoadingOrders" class="loading-inline">
      <div class="spinner-sm"></div>
      <span>Cargando pedidos...</span>
    </div>

    <!-- Empty state -->
    <div v-else-if="ordersStore.orders.length === 0" class="no-orders">
      No hay pedidos para este cliente.
    </div>

    <!-- Orders table -->
    <table v-else>
      <thead>
        <tr>
          <th>ID Pedido</th>
          <th>Fecha</th>
          <th>Estado</th>
          <th>Total</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <template v-for="order in ordersStore.orders" :key="order.id">
          <tr class="expandable-row" @click="toggleExpand(order.id)">
            <td>
              <span class="expand-icon">{{ expandedOrders.includes(order.id) ? '▼' : '▶' }}</span>
              {{ order.id.substring(0, 8) }}...
            </td>
            <td>{{ formatDate(order.createdAt) }}</td>
            <td><StatusBadge :status="order.status" /></td>
            <td class="font-bold">{{ formatMoney(order.totalAmount) }}</td>
            <td @click.stop>
              <div class="actions-cell">
                <!-- Dropdown de cambio de estado -->
                <select 
                  v-if="getNextStatusOptions(order.status).length > 0"
                  @change="(e) => handleStatusChange(order.id, (e.target as HTMLSelectElement).value)"
                  class="status-select"
                  :disabled="ordersStore.updating[order.id]"
                >
                  <option value="">Cambiar estado...</option>
                  <option 
                    v-for="option in getNextStatusOptions(order.status)" 
                    :key="option.statusId" 
                    :value="option.statusId"
                  >
                    {{ option.label }}
                  </option>
                </select>
                <span v-else class="text-sm text-gray-500 no-actions-text">Sin acciones</span>

                <!-- Botón de cancelar -->
                <button
                  @click="handleCancel(order.id)"
                  class="btn btn-danger btn-sm cancel-btn"
                  :disabled="!canCancel(order.status) || ordersStore.updating[order.id]"
                  :title="canCancel(order.status) ? 'Cancelar pedido' : 'Solo se pueden cancelar pedidos pendientes'"
                >
                  {{ ordersStore.updating[order.id] ? 'Actualizando...' : '✕ Cancelar' }}
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="expandedOrders.includes(order.id)" class="expanded-row">
            <td colspan="5" class="expanded-content">
              <div v-if="order.items && order.items.length > 0">
                <h4>Items del Pedido:</h4>
                <table class="items-table">
                  <thead>
                    <tr>
                      <th>Producto</th>
                      <th>Cantidad</th>
                      <th>Precio Unitario</th>
                      <th>Total Línea</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, idx) in order.items" :key="idx">
                      <td>{{ item.productName }}</td>
                      <td>{{ item.quantity }}</td>
                      <td>{{ formatMoney(item.unitPrice) }}</td>
                      <td class="font-bold">{{ formatMoney(item.lineTotal) }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div v-else class="no-items">
                No hay items en este pedido.
              </div>
            </td>
          </tr>
        </template>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useOrdersStore } from '@/stores/orders.store'
import StatusBadge from '@/components/StatusBadge.vue'
import { formatMoney, formatDate } from '@/utils/money'
import { getNextStatusOptions, canCancel } from '@/utils/orderStatus'

const props = defineProps<{
  customerId: string
}>()

const emit = defineEmits<{
  statusUpdated: []
}>()

const ordersStore = useOrdersStore()
const expandedOrders = ref<string[]>([])

onMounted(() => {
  ordersStore.fetchOrders(props.customerId)
})

watch(() => props.customerId, () => {
  ordersStore.fetchOrders(props.customerId)
})

function toggleExpand(orderId: string) {
  const index = expandedOrders.value.indexOf(orderId)
  
  if (index > -1) {
    expandedOrders.value.splice(index, 1)
  } else {
    expandedOrders.value.push(orderId)
  }
}

async function handleStatusChange(orderId: string, newStatusId: string) {
  if (!newStatusId) return
  
  const order = ordersStore.orders.find(o => o.id === orderId)
  if (!order) return
  
  const statusOption = getNextStatusOptions(order.status)
    .find(opt => opt.statusId === newStatusId)
  
  if (!statusOption) return
  
  const confirmed = confirm(`¿Confirmar: ${statusOption.label}?`)
  if (!confirmed) {
    // Reset select
    const selectElement = event?.target as HTMLSelectElement
    if (selectElement) {
      selectElement.value = ''
    }
    return
  }
  
  await ordersStore.changeStatus(props.customerId, orderId, newStatusId)
  emit('statusUpdated')
  
  // Reset select after update
  setTimeout(() => {
    const selectElement = document.querySelector(`select`) as HTMLSelectElement
    if (selectElement) {
      selectElement.value = ''
    }
  }, 100)
}

async function handleCancel(orderId: string) {
  const confirmed = confirm('¿Estás seguro de que deseas cancelar este pedido?')
  if (!confirmed) return
  
  await ordersStore.cancelOrder(props.customerId, orderId)
  emit('statusUpdated')
}
</script>

<style scoped>
.orders-container {
  margin-top: 1rem;
}

.no-orders {
  text-align: center;
  padding: 2rem;
  color: var(--gray-500);
  font-style: italic;
}

.no-items {
  text-align: center;
  padding: 1.5rem;
  color: var(--gray-500);
  font-style: italic;
}

.expand-icon {
  display: inline-block;
  width: 1rem;
  margin-right: 0.5rem;
  font-size: 0.75rem;
}

.actions-cell {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  flex-wrap: nowrap;
}

.status-select {
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  border-radius: var(--border-radius);
  border: 2px solid var(--gray-300);
  background-color: white;
  color: var(--gray-700);
  cursor: pointer;
  min-width: 180px;
  max-width: 200px;
  transition: all 0.2s ease;
  box-shadow: var(--shadow-sm);
}

.status-select:hover:not(:disabled) {
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.status-select:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.15);
}

.status-select:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  background-color: var(--gray-100);
}

.status-select option {
  padding: 0.5rem;
  font-size: 0.875rem;
}

.status-select option:first-child {
  color: var(--gray-500);
  font-style: italic;
}

.no-actions-text {
  display: inline-block;
  min-width: 100px;
}

.cancel-btn {
  white-space: nowrap;
}

.cancel-btn:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.close-btn {
  background: transparent;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: inherit;
  padding: 0;
  margin-left: 1rem;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.close-btn:hover {
  opacity: 1;
}

.expanded-row td {
  padding: 0;
}

.items-table {
  margin: 0;
  box-shadow: none;
}

.items-table th,
.items-table td {
  font-size: 0.8125rem;
}

h4 {
  margin-bottom: 0.75rem;
  color: var(--gray-700);
  font-size: 1rem;
}
</style>
