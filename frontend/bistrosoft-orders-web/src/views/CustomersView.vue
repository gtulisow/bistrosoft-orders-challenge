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
        <CustomerLookup @customer-found="handleCustomerFound" />
      </div>
    </div>

    <CustomerDetails 
      v-if="customerStore.currentCustomer"
      :customer="customerStore.currentCustomer"
    />

    <div class="card">
      <CustomersList 
        :customers="customerStore.createdCustomers"
        :loading="customerStore.loading"
        @select="handleCustomerSelect"
      />
    </div>

    <LoadingOverlay v-if="customerStore.loading" />
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useCustomerStore } from '@/stores/customer.store'
import CustomerForm from '@/components/CustomerForm.vue'
import CustomerLookup from '@/components/CustomerLookup.vue'
import CustomerDetails from '@/components/CustomerDetails.vue'
import CustomersList from '@/components/CustomersList.vue'
import LoadingOverlay from '@/components/LoadingOverlay.vue'
import ErrorBanner from '@/components/ErrorBanner.vue'
import type { CustomerDto } from '@/models/dtos'

const customerStore = useCustomerStore()

onMounted(() => {
  // Restaurar lista de clientes creados desde localStorage
  customerStore.restoreCreatedCustomers()
})

function handleCustomerCreated(customer: CustomerDto) {
  // Actualizar detalles del cliente actual
  customerStore.currentCustomer = customer
  customerStore.orders = customer.orders
  
  // Agregar a la lista de clientes creados
  customerStore.addCreatedCustomer(customer)
}

function handleCustomerFound(customer: CustomerDto) {
  customerStore.currentCustomer = customer
  customerStore.orders = customer.orders
}

async function handleCustomerSelect(customerId: string) {
  // Reutilizar el flujo existente de búsqueda por ID
  try {
    await customerStore.fetchCustomer(customerId)
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
