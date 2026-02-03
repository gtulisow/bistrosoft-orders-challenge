<template>
  <div class="alert alert-error">
    <span>⚠️ {{ message }}</span>
    <button @click="$emit('close')" class="close-btn">✕</button>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  message: string
}>()

defineEmits<{
  close: []
}>()
</script>

<style scoped>
.alert {
  display: flex;
  justify-content: space-between;
  align-items: center;
  white-space: pre-wrap;
}

.close-btn {
  background: transparent;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: inherit;
  padding: 0;
  margin-left: 1rem;
  opacity: 0.7;
  transition: opacity 0.2s;
}

.close-btn:hover {
  opacity: 1;
}
</style>
