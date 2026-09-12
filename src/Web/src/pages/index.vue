<template>
  <div class="flex items-center justify-center min-h-[60vh]">
    <div class="text-center">
      <h1 class="text-3xl font-bold text-gray-900 mb-4">Todo App</h1>
      <p class="text-gray-600 mb-8">Organize your tasks with style</p>
      <div class="flex gap-4 justify-center">
        <RouterLink
          to="/auth/login"
          class="px-6 py-3 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 font-medium"
        >
          Sign In
        </RouterLink>
        <RouterLink
          to="/auth/register"
          class="px-6 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 font-medium"
        >
          Create Account
        </RouterLink>
      </div>
    </div>
  </div>
</template>
