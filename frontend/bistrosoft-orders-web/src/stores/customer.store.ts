import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { CustomerDto, OrderSummaryDto } from '@/models/dtos'
import { customersApi } from '@/api/customers.api'
import { ordersApi } from '@/api/orders.api'

const STORAGE_KEY = 'customers.created.list.v1'
const MAX_CUSTOMERS = 50

export interface CustomerSummary {
  id: string
  name: string
  email: string
  phoneNumber?: string
  createdAtIso: string
}

export const useCustomerStore = defineStore('customer', () => {
  const currentCustomer = ref<CustomerDto | null>(null)
  const orders = ref<OrderSummaryDto[]>([])
  const createdCustomers = ref<CustomerSummary[]>([])
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

  function addCreatedCustomer(customer: CustomerDto) {
    // Validar que customer tenga las propiedades requeridas
    if (!customer || !customer.id || !customer.name || !customer.email) {
      console.error('Invalid customer data:', customer)
      return
    }

    const summary: CustomerSummary = {
      id: customer.id,
      name: customer.name,
      email: customer.email,
      phoneNumber: customer.phoneNumber || undefined,
      createdAtIso: new Date().toISOString()
    }

    // Evitar duplicados
    const exists = createdCustomers.value.some(c => c.id === customer.id)
    if (!exists) {
      // Agregar al inicio (más recientes primero)
      createdCustomers.value.unshift(summary)
      
      // Mantener máximo 50 clientes
      if (createdCustomers.value.length > MAX_CUSTOMERS) {
        createdCustomers.value = createdCustomers.value.slice(0, MAX_CUSTOMERS)
      }

      // Persistir en localStorage
      persistCreatedCustomers()
    }
  }

  function removeCreatedCustomer(id: string) {
    createdCustomers.value = createdCustomers.value.filter(c => c.id !== id)
    persistCreatedCustomers()
  }

  function persistCreatedCustomers() {
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(createdCustomers.value))
    } catch (err) {
      console.error('Error al guardar clientes en localStorage:', err)
    }
  }

  function restoreCreatedCustomers() {
    try {
      const stored = localStorage.getItem(STORAGE_KEY)
      if (stored) {
        const parsed = JSON.parse(stored)
        
        // Validar que sea un array y que cada item tenga las propiedades requeridas
        if (Array.isArray(parsed)) {
          createdCustomers.value = parsed.filter(customer => 
            customer &&
            customer.id &&
            customer.name &&
            customer.email &&
            customer.createdAtIso
          )
        } else {
          createdCustomers.value = []
        }
      }
    } catch (err) {
      console.error('Error al restaurar clientes de localStorage:', err)
      createdCustomers.value = []
      // Limpiar localStorage corrupto
      localStorage.removeItem(STORAGE_KEY)
    }
  }

  return {
    currentCustomer,
    orders,
    createdCustomers,
    loading,
    error,
    fetchCustomer,
    refreshOrders,
    clearCustomer,
    clearError,
    addCreatedCustomer,
    removeCreatedCustomer,
    restoreCreatedCustomers
  }
})
