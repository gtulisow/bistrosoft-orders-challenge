<template>
  <div class="container">
    <h1>Gestión de Clientes</h1>
    
    <ErrorBanner 
      v-if="customerStore.error" 
      :message="customerStore.error"
      @close="customerStore.clearError()"
    />

    <div class="grid grid-cols-2">
      <div class="card">
        <h2>Crear Nuevo Cliente</h2>
        <CustomerForm @customer-created="handleCustomerCreated" />
      </div>

      <div class="card">
        <h2>Buscar Cliente</h2>
        <CustomerLookup 
          ref="lookupRef"
          @customer-found="handleCustomerFound" 
        />
      </div>
    </div>

    <CustomerDetails 
      v-if="customerStore.currentCustomer"
      :customer="customerStore.currentCustomer"
      id="customer-details"
    />

    <div class="card">
      <CustomersList 
        :customers="customerStore.filteredCustomersList"
        :loading="customerStore.customersListLoading"
        :error="customerStore.customersListError"
        :search-text="customerStore.customersListSearch"
        :count="customerStore.customersListCount"
        @select="handleCustomerSelect"
        @update:search-text="customerStore.setCustomersListSearch"
      />
    </div>

    <LoadingOverlay v-if="customerStore.loading" />
    <Toast v-if="toastMessage" :message="toastMessage" type="success" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useCustomerStore } from '@/stores/customer.store'
import CustomerForm from '@/components/CustomerForm.vue'
import CustomerLookup from '@/components/CustomerLookup.vue'
import CustomerDetails from '@/components/CustomerDetails.vue'
import CustomersList from '@/components/CustomersList.vue'
import LoadingOverlay from '@/components/LoadingOverlay.vue'
import ErrorBanner from '@/components/ErrorBanner.vue'
import Toast from '@/components/Toast.vue'
import type { CustomerDto } from '@/models/dtos'

const customerStore = useCustomerStore()
const lookupRef = ref<InstanceType<typeof CustomerLookup> | null>(null)
const toastMessage = ref('')

onMounted(() => {
  // Cargar lista de clientes desde backend
  customerStore.fetchCustomersList()
})

async function handleCustomerCreated(customer: CustomerDto) {
  // Actualizar detalles del cliente actual
  customerStore.currentCustomer = customer
  customerStore.orders = customer.orders
  
  // Mostrar toast de éxito
  toastMessage.value = '✓ Cliente creado exitosamente'
  setTimeout(() => {
    toastMessage.value = ''
  }, 100)
  
  // Refrescar lista desde backend
  await customerStore.fetchCustomersList()
}

function handleCustomerFound(customer: CustomerDto) {
  customerStore.currentCustomer = customer
  customerStore.orders = customer.orders
}

async function handleCustomerSelect(customerId: string) {
  // Reutilizar el flujo existente de búsqueda por ID
  try {
    await customerStore.fetchCustomer(customerId)
    
    // Scroll suave a detalles
    setTimeout(() => {
      document.getElementById('customer-details')?.scrollIntoView({ 
        behavior: 'smooth',
        block: 'start'
      })
    }, 100)
  } catch (err) {
    // Error ya manejado en el store
  }
}
</script>

<style scoped>
h1 {
  margin-bottom: 2rem;
}

h2 {
  margin-bottom: 1.5rem;
}
</style>
