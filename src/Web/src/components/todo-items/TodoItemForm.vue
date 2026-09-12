<template>
  <form
    class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-4"
    @submit.prevent="handleSubmit"
  >
    <div class="space-y-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Title</label>
        <input
          v-model="form.title"
          type="text"
          required
          :class="[
            'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500',
            hasError('title') ? 'border-red-500' : 'border-gray-300',
          ]"
          placeholder="Task name"
          @blur="validate('title')"
        />
        <p v-if="hasError('title')" class="mt-1 text-sm text-red-600">{{ errors.title }}</p>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Note</label>
        <textarea
          v-model="form.note"
          rows="2"
          :class="[
            'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500',
            hasError('note') ? 'border-red-500' : 'border-gray-300',
          ]"
          placeholder="Optional note"
          @blur="validate('note')"
        />
        <p v-if="hasError('note')" class="mt-1 text-sm text-red-600">{{ errors.note }}</p>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Priority</label>
        <select
          v-model="form.priority"
          class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500"
        >
          <option :value="0">None</option>
          <option :value="1">Low</option>
          <option :value="2">Medium</option>
          <option :value="3">High</option>
        </select>
      </div>
    </div>

    <AlertBanner
      v-if="apiError"
      :message="apiError.message"
      :error="apiError.error"
      :severity="apiError.severity"
      dismissible
      class="mt-4"
      @dismiss="apiError = null"
    />

    <div class="flex gap-2 mt-4">
      <button
        type="submit"
        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 text-sm font-medium"
      >
        {{ item ? 'Update' : 'Add' }}
      </button>
      <button
        type="button"
        class="px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 text-sm font-medium"
        @click="$emit('cancel')"
      >
        Cancel
      </button>
    </div>
  </form>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import AlertBanner from '@/components/common/AlertBanner.vue'
import { useFormValidation } from '@/composables/useFormValidation'
import { parseApiError, type ApiErrorState } from '@/lib/errors'
import type { TodoItem } from '@/types/todoItem'

const props = defineProps<{
  listId: string
  item?: TodoItem | null
}>()

const emit = defineEmits<{
  submit: [data: { title: string; note?: string; priority?: number }]
  cancel: []
}>()

const form = reactive({
  title: props.item?.title ?? '',
  note: props.item?.note ?? '',
  priority: props.item?.priority ?? 0,
})

const apiError = ref<ApiErrorState | null>(null)

const { errors, validate, validateAll, hasError } = useFormValidation(form, {
  title: {
    required: true,
    minLength: 1,
    maxLength: 200,
    message: 'Title is required',
  },
  note: {
    maxLength: 500,
    message: 'Note must be under 500 characters',
  },
})

function handleSubmit() {
  if (!validateAll()) return

  apiError.value = null
  emit('submit', {
    title: form.title,
    note: form.note || undefined,
    priority: form.priority,
  })
}

function setError(error: unknown) {
  apiError.value = parseApiError(error)
}

defineExpose({ setError })
</script>
