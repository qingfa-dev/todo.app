<template>
  <div
    class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 hover:shadow-md transition-shadow cursor-pointer"
    @click="router.push(`/todo-lists/${list.id}`)"
  >
    <div class="flex items-start justify-between">
      <div class="flex items-center gap-2">
        <div
          class="w-3 h-3 rounded-full flex-shrink-0"
          :style="{ backgroundColor: list.colour }"
        />
        <h3 class="font-semibold text-gray-900">{{ list.title }}</h3>
      </div>
      <div class="flex items-center gap-1">
        <button
          class="text-gray-400 hover:text-indigo-600 p-1"
          title="Edit"
          @click.stop="$emit('edit', list)"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
          </svg>
        </button>
        <button
          class="text-gray-400 hover:text-red-600 p-1"
          title="Delete"
          @click.stop="$emit('delete', list.id)"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import type { TodoList } from '@/types/todoList'

defineProps<{ list: TodoList }>()
defineEmits<{
  edit: [list: TodoList]
  delete: [id: string]
}>()

const router = useRouter()
</script>
