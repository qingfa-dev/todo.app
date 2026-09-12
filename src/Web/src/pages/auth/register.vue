<template>
  <div class="flex items-center justify-center min-h-[60vh]">
    <div class="w-full max-w-md">
      <div class="bg-white rounded-lg shadow-md p-8">
        <h2 class="text-2xl font-bold text-gray-900 mb-6 text-center">Create Account</h2>

        <form @submit.prevent="handleRegister" class="space-y-4">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 mb-1">
              Email
            </label>
            <input
              id="email"
              v-model="form.email"
              type="email"
              required
              :class="[
                'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500',
                hasError('email') ? 'border-red-500' : 'border-gray-300',
              ]"
              placeholder="you@example.com"
              @blur="validate('email')"
            />
            <p v-if="hasError('email')" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-1">
              Password
            </label>
            <input
              id="password"
              v-model="form.password"
              type="password"
              required
              :class="[
                'w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500',
                hasError('password') ? 'border-red-500' : 'border-gray-300',
              ]"
              placeholder="Min 8 characters"
              @blur="validate('password')"
            />
            <p v-if="hasError('password')" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
          </div>

          <AlertBanner
            v-if="apiError"
            :message="apiError.message"
            :error="apiError.error"
            :severity="apiError.severity"
            dismissible
            @dismiss="apiError = null"
          />

          <button
            type="submit"
            :disabled="loading"
            class="w-full py-2 px-4 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 disabled:opacity-50 font-medium"
          >
            {{ loading ? 'Creating account...' : 'Create Account' }}
          </button>
        </form>

        <p class="mt-6 text-center text-sm text-gray-600">
          Already have an account?
          <RouterLink to="/auth/login" class="text-indigo-600 hover:text-indigo-800 font-medium">
            Sign in
          </RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useFormValidation } from '@/composables/useFormValidation'
import { parseApiError, type ApiErrorState } from '@/lib/errors'
import AlertBanner from '@/components/common/AlertBanner.vue'

const auth = useAuthStore()
const router = useRouter()

const form = reactive({ email: '', password: '' })
const loading = ref(false)
const apiError = ref<ApiErrorState | null>(null)

const { errors, validate, validateAll, hasError } = useFormValidation(form, {
  email: {
    required: true,
    pattern: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    message: 'Please enter a valid email address',
  },
  password: {
    required: true,
    minLength: 8,
    message: 'Password must be at least 8 characters',
  },
})

async function handleRegister() {
  if (!validateAll()) return

  loading.value = true
  apiError.value = null

  try {
    await auth.register(form)
    router.push('/todo-lists')
  } catch (e: unknown) {
    apiError.value = parseApiError(e)
  } finally {
    loading.value = false
  }
}
</script>
