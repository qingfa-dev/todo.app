<template>
  <div
    :class="[
      'rounded-lg border p-4 flex items-start gap-3',
      severityClasses.bg,
      severityClasses.border,
    ]"
    role="alert"
  >
    <svg
      :class="['w-5 h-5 flex-shrink-0 mt-0.5', severityClasses.icon]"
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
    >
      <path
        v-if="severity === 'error'"
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
      />
      <path
        v-else-if="severity === 'warning'"
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4.5c-.77-.833-2.694-.833-3.464 0L3.34 16.5c-.77.833.192 2.5 1.732 2.5z"
      />
      <path
        v-else
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
      />
    </svg>
    <div class="flex-1">
      <p :class="['text-sm font-medium', severityClasses.text]">{{ message }}</p>
      <p v-if="error?.title && error.title !== message" :class="['text-sm mt-1', severityClasses.text, 'opacity-75']">
        {{ error.title }}
      </p>
    </div>
    <button
      v-if="dismissible"
      :class="['flex-shrink-0 ml-2', severityClasses.text, 'hover:opacity-75']"
      @click="$emit('dismiss')"
    >
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ApiError } from '@/lib/errors'
import { getSeverityClasses } from '@/lib/errors'

const props = withDefaults(
  defineProps<{
    message: string
    error?: ApiError | null
    severity?: 'error' | 'warning' | 'info'
    dismissible?: boolean
  }>(),
  {
    error: null,
    severity: 'error',
    dismissible: false,
  },
)

defineEmits<{
  dismiss: []
}>()

const severityClasses = computed(() => getSeverityClasses(props.severity))
</script>
