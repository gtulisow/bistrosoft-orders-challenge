<template>
  <div class="card customer-details">
    <div class="card-header">
      <h2>Detalles del Cliente</h2>
      <button @click="copyCustomerId" class="btn btn-secondary btn-sm">
        📋 Copiar ID
      </button>
    </div>

    <div class="details-grid">
      <div class="detail-item">
        <span class="detail-label">ID:</span>
        <span class="detail-value">{{ customer.id }}</span>
      </div>
      <div class="detail-item">
        <span class="detail-label">Nombre:</span>
        <span class="detail-value">{{ customer.name }}</span>
      </div>
      <div class="detail-item">
        <span class="detail-label">Email:</span>
        <span class="detail-value">{{ customer.email }}</span>
      </div>
      <div class="detail-item">
        <span class="detail-label">Teléfono:</span>
        <span class="detail-value">{{ customer.phoneNumber || 'No proporcionado' }}</span>
      </div>
    </div>

    <div class="orders-summary">
      <h3>Resumen de Pedidos</h3>
      <div v-if="!customer.orders || customer.orders.length === 0" class="no-orders">
        No hay pedidos registrados para este cliente.
      </div>
      <div v-else class="orders-list">
        <div v-for="order in customer.orders" :key="order.id" class="order-summary-card">
          <div class="order-info">
            <div>
              <strong>Pedido #{{ order.id.substring(0, 8) }}</strong>
              <StatusBadge :status="order.status" />
            </div>
            <div class="text-sm text-gray-500">
              {{ formatDate(order.createdAt) }}
            </div>
          </div>
          <div class="order-amount">
            {{ formatMoney(order.totalAmount) }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { CustomerDto } from '@/models/dtos'
import StatusBadge from '@/components/StatusBadge.vue'
import { formatMoney, formatDate } from '@/utils/money'

defineProps<{
  customer: CustomerDto
}>()

async function copyCustomerId() {
  const customerId = (document.querySelector('.detail-value') as HTMLElement)?.textContent || ''
  try {
    await navigator.clipboard.writeText(customerId)
    alert('ID copiado al portapapeles')
  } catch (err) {
    console.error('Error al copiar:', err)
  }
}
</script>

<style scoped>
.customer-details {
  margin-top: 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.customer-details h2,
.customer-details h3 {
  color: white;
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1.5rem;
  margin: 1.5rem 0;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.detail-label {
  font-size: 0.875rem;
  opacity: 0.9;
  font-weight: 500;
}

.detail-value {
  font-size: 1.125rem;
  font-weight: 600;
}

.orders-summary {
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.2);
}

.no-orders {
  text-align: center;
  padding: 2rem;
  opacity: 0.8;
  font-style: italic;
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 1rem;
}

.order-summary-card {
  background: rgba(255, 255, 255, 0.15);
  backdrop-filter: blur(10px);
  padding: 1rem;
  border-radius: var(--border-radius);
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: transform 0.2s;
}

.order-summary-card:hover {
  transform: translateX(4px);
  background: rgba(255, 255, 255, 0.2);
}

.order-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.order-amount {
  font-size: 1.25rem;
  font-weight: 700;
}

@media (max-width: 768px) {
  .details-grid {
    grid-template-columns: 1fr;
  }
}
</style>
