<template>
  <Transition name="toast-slide">
    <div v-if="visible" class="toast" :class="toastClass">
      <span>{{ message }}</span>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue'

const props = defineProps<{
  message: string
  type?: 'success' | 'error' | 'info'
  duration?: number
}>()

const visible = ref(false)
const toastClass = computed(() => `toast-${props.type || 'success'}`)

let timeout: ReturnType<typeof setTimeout> | null = null

watch(() => props.message, (newMessage) => {
  if (newMessage) {
    visible.value = true
    
    if (timeout) clearTimeout(timeout)
    timeout = setTimeout(() => {
      visible.value = false
    }, props.duration || 1500)
  } else {
    visible.value = false
  }
})

// Auto-mostrar si message está presente al montar
onMounted(() => {
  if (props.message) {
    visible.value = true
    timeout = setTimeout(() => {
      visible.value = false
    }, props.duration || 1500)
  }
})
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

.toast-slide-enter-active,
.toast-slide-leave-active {
  transition: all 0.3s ease;
}

.toast-slide-enter-from {
  transform: translateY(100px);
  opacity: 0;
}

.toast-slide-leave-to {
  transform: translateY(100px);
  opacity: 0;
}
</style>
