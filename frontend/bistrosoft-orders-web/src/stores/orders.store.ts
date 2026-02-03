import { defineStore } from 'pinia'
import { ref, reactive } from 'vue'
import type { OrderSummaryDto } from '@/models/dtos'
import { ordersApi } from '@/api/orders.api'
import { OrderStatusIds } from '@/models/dtos'

export const useOrdersStore = defineStore('orders', () => {
  const orders = ref<OrderSummaryDto[]>([])
  const isLoadingOrders = ref(false)
  const ordersError = ref<string | null>(null)
  const successMessage = ref<string | null>(null)
  const updating = reactive<Record<string, boolean>>({})

  let successTimeout: ReturnType<typeof setTimeout> | null = null

  async function fetchOrders(customerId: string) {
    isLoadingOrders.value = true
    ordersError.value = null
    
    try {
      orders.value = await ordersApi.getCustomerOrders(customerId)
    } catch (err: any) {
      ordersError.value = err.message || 'Error al cargar los pedidos'
      orders.value = []
    } finally {
      isLoadingOrders.value = false
    }
  }

  async function changeStatus(customerId: string, orderId: string, newStatusId: string) {
    updating[orderId] = true
    ordersError.value = null
    successMessage.value = null
    
    // Limpiar timeout previo
    if (successTimeout) {
      clearTimeout(successTimeout)
      successTimeout = null
    }

    try {
      await ordersApi.updateOrderStatus(orderId, newStatusId)
      successMessage.value = '✓ Estado actualizado correctamente'
      
      // Auto-limpiar mensaje de éxito después de 3 segundos
      successTimeout = setTimeout(() => {
        successMessage.value = null
      }, 3000)
      
      // Recargar pedidos
      await fetchOrders(customerId)
    } catch (err: any) {
      ordersError.value = err.message || 'Error al actualizar el estado del pedido'
    } finally {
      updating[orderId] = false
    }
  }

  async function cancelOrder(customerId: string, orderId: string) {
    await changeStatus(customerId, orderId, OrderStatusIds.Cancelled)
  }

  function clearMessages() {
    ordersError.value = null
    successMessage.value = null
    if (successTimeout) {
      clearTimeout(successTimeout)
      successTimeout = null
    }
  }

  return {
    orders,
    isLoadingOrders,
    ordersError,
    successMessage,
    updating,
    fetchOrders,
    changeStatus,
    cancelOrder,
    clearMessages
  }
})
