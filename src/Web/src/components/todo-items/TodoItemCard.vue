<template>
  <div
    class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 flex items-center gap-4"
  >
    <input
      type="checkbox"
      :checked="item.done"
      class="w-5 h-5 text-indigo-600 rounded focus:ring-indigo-500"
      @change="$emit('toggleDone', item.id, item)"
    />

    <div class="flex-1 min-w-0">
      <p
        class="font-medium"
        :class="item.done ? 'text-gray-400 line-through' : 'text-gray-900'"
      >
        {{ item.title }}
      </p>
      <p v-if="item.note" class="text-sm text-gray-500 truncate">
        {{ item.note }}
      </p>
    </div>

    <span
      class="px-2 py-1 text-xs font-medium rounded-full"
      :class="priorityColor"
    >
      {{ priorityLabel }}
    </span>

    <div class="flex items-center gap-1">
      <button
        class="text-gray-400 hover:text-indigo-600 p-1"
        title="Edit"
        @click="$emit('edit', item)"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
        </svg>
      </button>
      <button
        class="text-gray-400 hover:text-red-600 p-1"
        title="Delete"
        @click="$emit('delete', item.id)"
      >
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { PriorityLevel, PRIORITY_LABELS, PRIORITY_COLORS } from '@/types/todoItem'
import type { TodoItem } from '@/types/todoItem'

const props = defineProps<{ item: TodoItem }>()
defineEmits<{
  toggleDone: [id: string, item: TodoItem]
  edit: [item: TodoItem]
  delete: [id: string]
}>()

const priorityLabel = computed(() => PRIORITY_LABELS[props.item.priority as PriorityLevel] ?? 'None')
const priorityColor = computed(() => PRIORITY_COLORS[props.item.priority as PriorityLevel] ?? 'bg-gray-100 text-gray-700')
</script>
