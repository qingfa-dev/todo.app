<template>
  <div class="flex flex-wrap gap-2">
    <button
      v-for="colour in SUPPORTED_COLOURS"
      :key="colour.code"
      type="button"
      class="w-8 h-8 rounded-full border-2 transition-all"
      :class="modelValue === colour.code ? 'border-gray-900 scale-110' : 'border-transparent'"
      :style="{ backgroundColor: colour.code }"
      :title="colour.name"
      @click="$emit('update:modelValue', colour.code)"
    />
  </div>
</template>

<script setup lang="ts">
import { SUPPORTED_COLOURS } from '@/types/todoList'

defineProps<{ modelValue: string }>()
defineEmits<{ 'update:modelValue': [value: string] }>()
</script>
