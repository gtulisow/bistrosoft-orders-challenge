<template>
  <span :class="['badge', badgeClass]">
    {{ statusLabel }}
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { OrderStatusDto } from '@/models/dtos'
import { statusToLabel, statusBadgeClass } from '@/utils/orderStatus'

const props = defineProps<{
  status: OrderStatusDto
}>()

const statusLabel = computed(() => statusToLabel(props.status))
const badgeClass = computed(() => statusBadgeClass(props.status))
</script>

<style scoped>
/* Los estilos de badge ya están definidos en style.css global */
</style>
