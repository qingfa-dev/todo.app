<template>
  <header class="bg-white shadow-sm border-b border-gray-200">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <RouterLink to="/todo-lists" class="text-xl font-bold text-indigo-600">
          Todo App
        </RouterLink>

        <nav v-if="auth.isAuthenticated" class="flex items-center gap-4">
          <RouterLink
            to="/todo-lists"
            class="text-gray-600 hover:text-gray-900 text-sm font-medium"
          >
            My Lists
          </RouterLink>
          <span class="text-sm text-gray-500">{{ auth.user?.email }}</span>
          <button
            class="text-sm text-red-600 hover:text-red-800 font-medium"
            @click="handleLogout"
          >
            Logout
          </button>
        </nav>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()
const router = useRouter()

async function handleLogout() {
  await auth.logout()
  router.push('/auth/login')
}
</script>
