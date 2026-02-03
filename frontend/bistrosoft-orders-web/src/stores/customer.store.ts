import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { CustomerDto, OrderSummaryDto } from '@/models/dtos'
import { customersApi } from '@/api/customers.api'
import { ordersApi } from '@/api/orders.api'

export const useCustomerStore = defineStore('customer', () => {
  const currentCustomer = ref<CustomerDto | null>(null)
  const orders = ref<OrderSummaryDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchCustomer(id: string) {
    loading.value = true
    error.value = null
    
    try {
      currentCustomer.value = await customersApi.getById(id)
      orders.value = currentCustomer.value.orders
    } catch (err: any) {
      error.value = err.message || 'Error al cargar el cliente'
      currentCustomer.value = null
      orders.value = []
      throw err
    } finally {
      loading.value = false
    }
  }

  async function refreshOrders(customerId: string) {
    try {
      orders.value = await ordersApi.getCustomerOrders(customerId)
    } catch (err: any) {
      error.value = err.message || 'Error al cargar los pedidos'
      throw err
    }
  }

  function clearCustomer() {
    currentCustomer.value = null
    orders.value = []
    error.value = null
  }

  function clearError() {
    error.value = null
  }

  return {
    currentCustomer,
    orders,
    loading,
    error,
    fetchCustomer,
    refreshOrders,
    clearCustomer,
    clearError
  }
})
