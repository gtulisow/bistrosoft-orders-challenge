<template>
  <div v-show="message" class="toast" :class="toastClass">
    <span>{{ message }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  message: string
  type?: 'success' | 'error' | 'info'
  duration?: number
}>()

const toastClass = computed(() => `toast-${props.type || 'success'}`)
</script>

<style scoped>
.toast {
  position: fixed;
  bottom: 2rem;
  right: 2rem;
  padding: 1rem 1.5rem;
  border-radius: var(--border-radius);
  box-shadow: var(--shadow-lg);
  font-weight: 500;
  z-index: 10000;
  min-width: 200px;
  text-align: center;
  animation: slideIn 0.3s ease;
}

.toast-success {
  background-color: var(--success-color);
  color: white;
}

.toast-error {
  background-color: var(--danger-color);
  color: white;
}

.toast-info {
  background-color: var(--info-color);
  color: white;
}

@keyframes slideIn {
  from {
    transform: translateY(100px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}
</style>
