import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

import HomeView from '@/pages/index.vue'
import LoginView from '@/pages/auth/login.vue'
import RegisterView from '@/pages/auth/register.vue'
import TodoListsIndex from '@/pages/todo-lists/index.vue'
import TodoListDetail from '@/pages/todo-lists/[id].vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('@/layouts/default.vue'),
      children: [
        { path: '', name: 'home', component: HomeView, meta: { middleware: 'guest' } },
        { path: 'todo-lists', name: 'todo-lists', component: TodoListsIndex, meta: { middleware: 'auth' } },
        { path: 'todo-lists/:id', name: 'todo-list-detail', component: TodoListDetail, meta: { middleware: 'auth' } },
      ],
    },
    {
      path: '/auth',
      component: () => import('@/layouts/auth.vue'),
      children: [
        { path: 'login', name: 'login', component: LoginView, meta: { middleware: 'guest' } },
        { path: 'register', name: 'register', component: RegisterView, meta: { middleware: 'guest' } },
      ],
    },
  ],
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.middleware === 'auth' && !auth.isAuthenticated) {
    return { name: 'login' }
  }

  if (to.meta.middleware === 'guest' && auth.isAuthenticated) {
    return { name: 'todo-lists' }
  }
})

export default router
