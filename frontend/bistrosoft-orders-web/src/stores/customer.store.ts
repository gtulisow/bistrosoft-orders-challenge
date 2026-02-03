import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { CustomerDto, OrderSummaryDto, CustomerListDto } from '@/models/dtos'
import { customersApi } from '@/api/customers.api'
import { ordersApi } from '@/api/orders.api'

export const useCustomerStore = defineStore('customer', () => {
  // Estado para detalle de cliente (flujo existente)
  const currentCustomer = ref<CustomerDto | null>(null)
  const orders = ref<OrderSummaryDto[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Estado para lista de clientes
  const customersList = ref<CustomerListDto[]>([])
  const customersListLoading = ref(false)
  const customersListError = ref<string | null>(null)
  const customersListSearch = ref('')

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

  // Computed: lista filtrada de clientes
  const filteredCustomersList = computed(() => {
    if (!customersListSearch.value) {
      return customersList.value
    }

    const search = customersListSearch.value.toLowerCase()
    return customersList.value.filter(customer =>
      customer.name?.toLowerCase().includes(search) ||
      customer.email?.toLowerCase().includes(search) ||
      customer.id?.toLowerCase().includes(search) ||
      customer.phoneNumber?.toLowerCase().includes(search)
    )
  })

  const customersListCount = computed(() => filteredCustomersList.value.length)

  // Acción: obtener lista de clientes desde backend
  async function fetchCustomersList() {
    customersListLoading.value = true
    customersListError.value = null

    try {
      customersList.value = await customersApi.getAll()
    } catch (err: any) {
      customersListError.value = err.message || 'Error al cargar la lista de clientes'
      customersList.value = []
    } finally {
      customersListLoading.value = false
    }
  }

  function setCustomersListSearch(value: string) {
    customersListSearch.value = value
  }

  return {
    currentCustomer,
    orders,
    customersList,
    filteredCustomersList,
    customersListCount,
    customersListLoading,
    customersListError,
    customersListSearch,
    loading,
    error,
    fetchCustomer,
    refreshOrders,
    fetchCustomersList,
    setCustomersListSearch,
    clearCustomer,
    clearError
  }
})
