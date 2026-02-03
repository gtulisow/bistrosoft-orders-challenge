<template>
  <div class="customers-list-container">
    <div class="list-header">
      <h2>Clientes Creados</h2>
      <div class="filter-section">
        <input
          v-model="filterText"
          type="text"
          placeholder="🔍 Buscar por nombre, email o ID..."
          class="filter-input"
        />
        <span class="text-sm text-gray-500">{{ filteredCustomers.length }} cliente(s)</span>
      </div>
    </div>

    <div v-if="filteredCustomers.length === 0" class="empty-state">
      <div class="empty-icon">👥</div>
      <p v-if="!filterText">Aún no hay clientes. Creá el primero arriba.</p>
      <p v-else>No se encontraron clientes con "{{ filterText }}"</p>
    </div>

    <div v-else class="customers-grid">
      <div
        v-for="(customer, index) in filteredCustomers"
        :key="customer.id || `customer-${index}`"
        class="customer-card"
      >
        <div class="customer-main">
          <div class="customer-info">
            <h3>{{ customer.name || 'Sin nombre' }}</h3>
            <p class="email">{{ customer.email || 'Sin email' }}</p>
            <p v-if="customer.phoneNumber" class="phone">📞 {{ customer.phoneNumber }}</p>
            <p v-if="customer.id" class="customer-id">
              ID: {{ customer.id.substring(0, 8) }}...
            </p>
            <p class="created-at">{{ formatRelativeDate(customer.createdAtIso) }}</p>
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
            👁️ Ver Detalle
          </button>
        </div>
      </div>
    </div>

    <Toast v-if="toastMessage" :message="toastMessage" type="success" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { CustomerSummary } from '@/stores/customer.store'
import Toast from '@/components/Toast.vue'

const props = defineProps<{
  customers: CustomerSummary[]
  loading?: boolean
}>()

const emit = defineEmits<{
  select: [id: string]
}>()

const filterText = ref('')
const toastMessage = ref('')

const filteredCustomers = computed(() => {
  if (!props.customers || props.customers.length === 0) {
    return []
  }

  if (!filterText.value) {
    return props.customers
  }

  const search = filterText.value.toLowerCase()
  return props.customers.filter(customer => {
    if (!customer) return false
    
    return (
      customer.name?.toLowerCase().includes(search) ||
      customer.email?.toLowerCase().includes(search) ||
      customer.id?.toLowerCase().includes(search) ||
      customer.phoneNumber?.toLowerCase().includes(search)
    )
  })
})

async function handleCopyId(id: string) {
  try {
    await navigator.clipboard.writeText(id)
    toastMessage.value = '✓ ID copiado al portapapeles'
    
    setTimeout(() => {
      toastMessage.value = ''
    }, 100)
  } catch (err) {
    console.error('Error al copiar:', err)
  }
}

function formatRelativeDate(isoString: string): string {
  const date = new Date(isoString)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMs / 3600000)
  const diffDays = Math.floor(diffMs / 86400000)

  if (diffMins < 1) return 'Hace un momento'
  if (diffMins < 60) return `Hace ${diffMins} minuto${diffMins > 1 ? 's' : ''}`
  if (diffHours < 24) return `Hace ${diffHours} hora${diffHours > 1 ? 's' : ''}`
  if (diffDays < 7) return `Hace ${diffDays} día${diffDays > 1 ? 's' : ''}`
  
  return new Intl.DateTimeFormat('es-MX', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(date)
}
</script>

<style scoped>
.customers-list-container {
  margin-top: 2rem;
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
  margin-bottom: 0.5rem;
}

.created-at {
  font-size: 0.75rem;
  color: var(--gray-500);
  font-style: italic;
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
