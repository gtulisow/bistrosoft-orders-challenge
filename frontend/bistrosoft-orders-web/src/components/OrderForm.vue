<template>
  <form @submit.prevent="handleSubmit">
    <div class="form-group">
      <label for="customer-id">Customer ID *</label>
      <input 
        id="customer-id"
        v-model="form.customerId" 
        type="text" 
        placeholder="ID del cliente"
        required
        :disabled="loading"
      />
    </div>

    <div class="items-section">
      <div class="section-header">
        <h3>Productos del Pedido</h3>
        <button 
          type="button" 
          @click="addItem" 
          class="btn btn-secondary btn-sm"
          :disabled="loading"
        >
          ➕ Agregar Producto
        </button>
      </div>

      <div v-for="(item, index) in form.items" :key="index" class="item-row">
        <div class="form-group" style="flex: 2;">
          <label>Producto</label>
          <select v-model="item.productId" required :disabled="loading">
            <option value="">Selecciona un producto</option>
            <option v-for="product in productStore.products" :key="product.id" :value="product.id">
              {{ product.name }} - {{ formatMoney(product.price) }}
            </option>
          </select>
        </div>

        <div class="form-group" style="flex: 1;">
          <label>Cantidad</label>
          <input 
            v-model.number="item.quantity" 
            type="number" 
            min="1" 
            required
            :disabled="loading"
          />
        </div>

        <button 
          type="button" 
          @click="removeItem(index)" 
          class="btn btn-danger btn-sm remove-btn"
          :disabled="loading || form.items.length === 1"
        >
          🗑️
        </button>
      </div>
    </div>

    <div v-if="estimatedTotal > 0" class="total-preview">
      <strong>Total estimado:</strong> {{ formatMoney(estimatedTotal) }}
      <span class="text-sm text-gray-500">(calculado en el cliente)</span>
    </div>

    <div v-if="error" class="alert alert-error">
      {{ error }}
    </div>

    <div v-if="success" class="alert alert-success">
      ✓ Pedido creado exitosamente con ID: <strong>{{ createdOrderId }}</strong>
    </div>

    <button type="submit" class="btn btn-primary" :disabled="loading || !isFormValid">
      {{ loading ? 'Creando...' : '🛒 Crear Pedido' }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { ordersApi } from '@/api/orders.api'
import { useProductStore } from '@/stores/product.store'
import { formatMoney } from '@/utils/money'
import type { OrderItemInput } from '@/models/dtos'

const emit = defineEmits<{
  orderCreated: [customerId: string]
}>()

const productStore = useProductStore()

const form = reactive({
  customerId: '',
  items: [
    { productId: '', quantity: 1 }
  ] as OrderItemInput[]
})

const loading = ref(false)
const error = ref<string | null>(null)
const success = ref(false)
const createdOrderId = ref<string>('')

const isFormValid = computed(() => {
  return form.customerId.trim() !== '' && 
         form.items.length > 0 && 
         form.items.every(item => item.productId !== '' && item.quantity > 0)
})

const estimatedTotal = computed(() => {
  return form.items.reduce((total, item) => {
    const price = productStore.getProductPrice(item.productId)
    return total + (price * item.quantity)
  }, 0)
})

function addItem() {
  form.items.push({ productId: '', quantity: 1 })
}

function removeItem(index: number) {
  if (form.items.length > 1) {
    form.items.splice(index, 1)
  }
}

async function handleSubmit() {
  if (!isFormValid.value) return

  loading.value = true
  error.value = null
  success.value = false

  try {
    const order = await ordersApi.create({
      customerId: form.customerId.trim(),
      items: form.items
    })

    createdOrderId.value = order.id
    success.value = true
    
    emit('orderCreated', form.customerId.trim())
    
    // Limpiar formulario
    form.customerId = ''
    form.items = [{ productId: '', quantity: 1 }]
  } catch (err: any) {
    error.value = err.message || 'Error al crear el pedido'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.items-section {
  border: 2px dashed var(--gray-300);
  padding: 1.5rem;
  border-radius: var(--border-radius);
  background-color: var(--gray-50);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.section-header h3 {
  margin: 0;
}

.item-row {
  display: flex;
  gap: 1rem;
  align-items: flex-end;
  margin-bottom: 1rem;
  padding: 1rem;
  background: white;
  border-radius: var(--border-radius);
}

.item-row:last-child {
  margin-bottom: 0;
}

.remove-btn {
  margin-bottom: 1rem;
  padding: 0.625rem;
}

.total-preview {
  padding: 1rem;
  background-color: var(--gray-100);
  border-radius: var(--border-radius);
  text-align: center;
  font-size: 1.125rem;
}

.total-preview strong {
  color: var(--primary-color);
}
</style>
