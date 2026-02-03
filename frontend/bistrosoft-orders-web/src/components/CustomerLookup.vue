<template>
  <form @submit.prevent="handleSearch">
    <div class="form-group">
      <label for="customer-id">Customer ID</label>
      <input 
        id="customer-id"
        v-model="customerId" 
        type="text" 
        placeholder="Ingresa el ID del cliente"
        required
        :disabled="loading"
      />
    </div>

    <div v-if="error" class="alert alert-error">
      {{ error }}
    </div>

    <button type="submit" class="btn btn-primary" :disabled="loading || !customerId">
      {{ loading ? 'Buscando...' : '🔍 Buscar Cliente' }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useCustomerStore } from '@/stores/customer.store'
import type { CustomerDto } from '@/models/dtos'

const emit = defineEmits<{
  customerFound: [customer: CustomerDto]
}>()

const customerStore = useCustomerStore()
const customerId = ref('')
const loading = ref(false)
const error = ref<string | null>(null)

async function handleSearch() {
  if (!customerId.value.trim()) return

  loading.value = true
  error.value = null

  try {
    await customerStore.fetchCustomer(customerId.value.trim())
    
    if (customerStore.currentCustomer) {
      emit('customerFound', customerStore.currentCustomer)
    }
  } catch (err: any) {
    error.value = err.message || 'Cliente no encontrado'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}
</style>
