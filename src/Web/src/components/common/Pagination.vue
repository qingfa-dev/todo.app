<template>
  <div class="flex items-center justify-center gap-2 mt-8">
    <button
      :disabled="!hasPrevious"
      class="px-3 py-1 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
      @click="$emit('pageChange', page - 1)"
    >
      Previous
    </button>

    <span class="text-sm text-gray-600">
      Page {{ page }} of {{ totalPages }}
    </span>

    <button
      :disabled="!hasNext"
      class="px-3 py-1 text-sm border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
      @click="$emit('pageChange', page + 1)"
    >
      Next
    </button>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  page: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}>()

defineEmits<{ pageChange: [page: number] }>()
</script>
