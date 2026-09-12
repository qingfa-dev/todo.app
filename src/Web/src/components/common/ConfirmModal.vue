<template>
  <Teleport to="body">
    <div
      class="fixed inset-0 z-50 flex items-center justify-center"
      @keydown.esc="$emit('cancel')"
    >
      <div
        class="fixed inset-0 bg-black/50 backdrop-blur-sm transition-opacity"
        @click="$emit('cancel')"
      />
      <div
        class="relative z-10 w-full max-w-sm mx-4 bg-white rounded-xl shadow-2xl border border-gray-200"
        role="alertdialog"
        aria-modal="true"
      >
        <div class="p-6">
          <div class="flex items-center gap-4 mb-4">
            <div class="flex-shrink-0 w-10 h-10 rounded-full bg-red-100 flex items-center justify-center">
              <svg class="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </div>
            <div>
              <h3 class="text-lg font-semibold text-gray-900">{{ title }}</h3>
              <p class="text-sm text-gray-600 mt-1">{{ message }}</p>
            </div>
          </div>
          <div class="flex gap-3 justify-end">
            <button
              class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
              @click="$emit('cancel')"
            >
              Cancel
            </button>
            <button
              class="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 transition-colors"
              @click="$emit('confirm')"
            >
              {{ confirmText }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title: string
    message: string
    confirmText?: string
  }>(),
  {
    confirmText: 'Delete',
  },
)

defineEmits<{
  confirm: []
  cancel: []
}>()
</script>
