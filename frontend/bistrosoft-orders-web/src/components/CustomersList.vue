<template>
  <div class="customers-list-container">
    <div class="list-header">
      <h2>Clientes Registrados</h2>
      <div class="filter-section">
        <input
          :value="searchText"
          @input="handleSearchInput"
          type="text"
          placeholder="🔍 Buscar por nombre, email o ID..."
          class="filter-input"
          :disabled="loading"
        />
        <span class="text-sm text-gray-500">{{ count }} cliente(s)</span>
      </div>
    </div>

    <div v-if="error" class="alert alert-error">
      {{ error }}
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner-sm"></div>
      <span>Cargando clientes...</span>
    </div>

    <div v-else-if="customers.length === 0" class="empty-state">
      <div class="empty-icon">👥</div>
      <p v-if="!searchText">Aún no hay clientes registrados. Creá el primero arriba.</p>
      <p v-else>No se encontraron clientes con "{{ searchText }}"</p>
    </div>

    <div v-else class="customers-grid">
      <div
        v-for="customer in customers"
        :key="customer.id"
        class="customer-card"
      >
        <div class="customer-main">
          <div class="customer-info">
            <h3>{{ customer.name }}</h3>
            <p class="email">{{ customer.email }}</p>
            <p v-if="customer.phoneNumber" class="phone">📞 {{ customer.phoneNumber }}</p>
            <p class="customer-id">
              ID: {{ customer.id.substring(0, 8) }}...
            </p>
          </div>
        </div>
        
        <div class="customer-actions">
          <button
            @click="handleCopyId(customer.id)"
            class="btn btn-secondary btn-sm"
            :disabled="loading"
          >
            📋 Copiar ID
          </button>
          <button
            @click="$emit('select', customer.id)"
            class="btn btn-primary btn-sm"
            :disabled="loading"
          >
            👁️ Usar
          </button>
        </div>
      </div>
    </div>

    <Toast v-if="toastMessage" :message="toastMessage" type="success" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { CustomerListDto } from '@/models/dtos'
import Toast from '@/components/Toast.vue'

defineProps<{
  customers: CustomerListDto[]
  loading?: boolean
  error?: string | null
  searchText?: string
  count?: number
}>()

const emit = defineEmits<{
  select: [id: string]
  'update:searchText': [value: string]
}>()

const toastMessage = ref('')
let searchDebounce: ReturnType<typeof setTimeout> | null = null

function handleSearchInput(event: Event) {
  const value = (event.target as HTMLInputElement).value
  
  if (searchDebounce) clearTimeout(searchDebounce)
  
  searchDebounce = setTimeout(() => {
    emit('update:searchText', value)
  }, 250)
}

async function handleCopyId(id: string) {
  try {
    await navigator.clipboard.writeText(id)
    toastMessage.value = '✓ ID copiado al portapapeles'
    
    setTimeout(() => {
      toastMessage.value = ''
    }, 100)
  } catch (err) {
    // Fallback con textarea
    const textarea = document.createElement('textarea')
    textarea.value = id
    textarea.style.position = 'fixed'
    textarea.style.opacity = '0'
    document.body.appendChild(textarea)
    textarea.select()
    document.execCommand('copy')
    document.body.removeChild(textarea)
    
    toastMessage.value = '✓ ID copiado'
    setTimeout(() => {
      toastMessage.value = ''
    }, 100)
  }
}
</script>

<style scoped>
.customers-list-container {
  margin-top: 0;
}

.list-header {
  margin-bottom: 1.5rem;
}

.list-header h2 {
  margin-bottom: 1rem;
}

.filter-section {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.filter-input {
  flex: 1;
  max-width: 400px;
  padding: 0.625rem 1rem;
  border: 2px solid var(--gray-300);
  border-radius: var(--border-radius);
  font-size: 0.875rem;
  transition: border-color 0.2s;
}

.filter-input:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.filter-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 3rem;
  color: var(--gray-600);
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  background-color: var(--gray-50);
  border: 2px dashed var(--gray-300);
  border-radius: var(--border-radius);
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-state p {
  color: var(--gray-600);
  font-size: 1rem;
}

.customers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1.5rem;
}

.customer-card {
  background: white;
  border-radius: var(--border-radius);
  padding: 1.5rem;
  box-shadow: var(--shadow-sm);
  transition: all 0.2s;
  border: 1px solid var(--gray-200);
}

.customer-card:hover {
  box-shadow: var(--shadow-md);
  transform: translateY(-2px);
  border-color: var(--primary-color);
}

.customer-main {
  margin-bottom: 1rem;
}

.customer-info h3 {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--gray-900);
  margin-bottom: 0.5rem;
}

.email {
  color: var(--gray-700);
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.phone {
  color: var(--gray-600);
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.customer-id {
  font-family: 'Courier New', monospace;
  font-size: 0.75rem;
  color: var(--gray-500);
  background-color: var(--gray-100);
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  display: inline-block;
  margin-top: 0.5rem;
}

.customer-actions {
  display: flex;
  gap: 0.5rem;
  padding-top: 1rem;
  border-top: 1px solid var(--gray-200);
}

.customer-actions button {
  flex: 1;
}

@media (max-width: 768px) {
  .customers-grid {
    grid-template-columns: 1fr;
  }
  
  .filter-section {
    flex-direction: column;
    align-items: stretch;
  }
  
  .filter-input {
    max-width: 100%;
  }
}
</style>
