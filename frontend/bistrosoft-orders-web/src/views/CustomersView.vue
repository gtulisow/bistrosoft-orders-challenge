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

    <LoadingOverlay v-if="customerStore.loading" />
  </div>
</template>

<script setup lang="ts">
import { useCustomerStore } from '@/stores/customer.store'
import CustomerForm from '@/components/CustomerForm.vue'
import CustomerLookup from '@/components/CustomerLookup.vue'
import CustomerDetails from '@/components/CustomerDetails.vue'
import LoadingOverlay from '@/components/LoadingOverlay.vue'
import ErrorBanner from '@/components/ErrorBanner.vue'
import type { CustomerDto } from '@/models/dtos'

const customerStore = useCustomerStore()

function handleCustomerCreated(customer: CustomerDto) {
  customerStore.currentCustomer = customer
  customerStore.orders = customer.orders
}

function handleCustomerFound(customer: CustomerDto) {
  customerStore.currentCustomer = customer
  customerStore.orders = customer.orders
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
