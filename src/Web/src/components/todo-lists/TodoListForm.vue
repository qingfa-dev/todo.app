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
          placeholder="List name"
          @blur="validate('title')"
        />
        <p v-if="hasError('title')" class="mt-1 text-sm text-red-600">{{ errors.title }}</p>
      </div>

      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Colour</label>
        <ColorPicker v-model="form.colour" />
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
        {{ list ? 'Update' : 'Save' }}
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
import ColorPicker from '@/components/common/ColorPicker.vue'
import AlertBanner from '@/components/common/AlertBanner.vue'
import { useFormValidation } from '@/composables/useFormValidation'
import { parseApiError, type ApiErrorState } from '@/lib/errors'
import type { TodoList } from '@/types/todoList'

const props = defineProps<{
  list?: TodoList | null
}>()

const emit = defineEmits<{
  submit: [data: { title: string; colour: string }]
  cancel: []
}>()

const form = reactive({
  title: props.list?.title ?? '',
  colour: props.list?.colour ?? '#78909C',
})

const apiError = ref<ApiErrorState | null>(null)

const { errors, validate, validateAll, hasError } = useFormValidation(form, {
  title: {
    required: true,
    minLength: 1,
    maxLength: 100,
    message: 'Title is required',
  },
})

function handleSubmit() {
  if (!validateAll()) return

  apiError.value = null
  emit('submit', { ...form })
}

function setError(error: unknown) {
  apiError.value = parseApiError(error)
}

defineExpose({ setError })
</script>
