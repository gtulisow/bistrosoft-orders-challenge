<template>
  <form @submit.prevent="handleSubmit">
    <div class="form-group">
      <label for="name">Nombre *</label>
      <input 
        id="name"
        v-model="form.name" 
        type="text" 
        placeholder="Juan Pérez"
        required
        :disabled="isFormDisabled"
      />
      <span v-if="errors.name" class="form-error">{{ errors.name }}</span>
    </div>

    <div class="form-group">
      <label for="email">Email *</label>
      <input 
        id="email"
        v-model="form.email" 
        type="email" 
        placeholder="juan@example.com"
        required
        :disabled="isFormDisabled"
      />
      <span v-if="errors.email" class="form-error">{{ errors.email }}</span>
    </div>

    <div class="form-group">
      <label for="phone">Teléfono (opcional)</label>
      <input 
        id="phone"
        v-model="form.phoneNumber" 
        type="tel" 
        placeholder="+52 555-1234-5678"
        :disabled="isFormDisabled"
      />
      <span v-if="errors.phoneNumber" class="form-error">{{ errors.phoneNumber }}</span>
    </div>

    <div v-if="error" class="alert alert-error">
      {{ error }}
    </div>

    <div v-if="success" class="alert alert-success">
      ✓ Cliente creado exitosamente con ID: <strong>{{ createdCustomerId }}</strong>
      <button @click="copyCustomerId" class="copy-btn" type="button">
        📋 Copiar ID
      </button>
    </div>

    <button 
      type="submit" 
      class="btn btn-primary" 
      :class="{ 'btn-success': isSuccess }"
      :disabled="isFormDisabled"
    >
      {{ buttonText }}
    </button>
  </form>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { customersApi } from '@/api/customers.api'
import { isValidEmail, isValidPhone, isNotEmpty } from '@/utils/validators'
import type { CustomerDto } from '@/models/dtos'

const emit = defineEmits<{
  customerCreated: [customer: CustomerDto]
}>()

const form = reactive({
  name: '',
  email: '',
  phoneNumber: ''
})

const errors = reactive({
  name: '',
  email: '',
  phoneNumber: ''
})

// Estados explícitos para el flujo async
const isSubmitting = ref(false)
const isSuccess = ref(false)
const error = ref<string | null>(null)
const success = ref(false)
const createdCustomerId = ref<string>('')

// Computed para el texto del botón
const buttonText = computed(() => {
  if (isSuccess.value) return '✓ Creado con éxito'
  if (isSubmitting.value) return 'Creando...'
  return 'Crear Cliente'
})

// Computed para deshabilitar inputs y botón
const isFormDisabled = computed(() => isSubmitting.value || isSuccess.value)

function validateForm(): boolean {
  let isValid = true
  
  errors.name = ''
  errors.email = ''
  errors.phoneNumber = ''

  if (!isNotEmpty(form.name)) {
    errors.name = 'El nombre es requerido'
    isValid = false
  }

  if (!isNotEmpty(form.email)) {
    errors.email = 'El email es requerido'
    isValid = false
  } else if (!isValidEmail(form.email)) {
    errors.email = 'El email no es válido'
    isValid = false
  }

  if (form.phoneNumber && !isValidPhone(form.phoneNumber)) {
    errors.phoneNumber = 'El teléfono no es válido'
    isValid = false
  }

  return isValid
}

function resetForm() {
  form.name = ''
  form.email = ''
  form.phoneNumber = ''
  errors.name = ''
  errors.email = ''
  errors.phoneNumber = ''
}

async function handleSubmit() {
  if (!validateForm()) return

  // Estado: Submitting
  isSubmitting.value = true
  isSuccess.value = false
  error.value = null
  success.value = false

  try {
    const customer = await customersApi.create({
      name: form.name,
      email: form.email,
      phoneNumber: form.phoneNumber || undefined
    })

    createdCustomerId.value = customer.id
    success.value = true
    
    // Emitir evento para el padre
    emit('customerCreated', customer)
    
    // Estado: Success
    isSubmitting.value = false
    isSuccess.value = true

    // Después de 2 segundos: limpiar y resetear
    setTimeout(() => {
      resetForm()
      isSuccess.value = false
      success.value = false
      createdCustomerId.value = ''
    }, 2000)

  } catch (err: any) {
    // Estado: Error - restaurar botón inmediatamente
    error.value = err.message || 'Error al crear el cliente'
    isSubmitting.value = false
    isSuccess.value = false
  }
}

async function copyCustomerId() {
  try {
    await navigator.clipboard.writeText(createdCustomerId.value)
    alert('ID copiado al portapapeles')
  } catch (err) {
    console.error('Error al copiar:', err)
  }
}
</script>

<style scoped>
form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.copy-btn {
  margin-left: 0.5rem;
}

/* Estado de éxito del botón */
.btn-success {
  background-color: var(--success-color) !important;
  transition: background-color 0.3s ease;
}

.btn-success:hover {
  background-color: var(--success-color) !important;
}
</style>
