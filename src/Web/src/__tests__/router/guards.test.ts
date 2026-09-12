import { describe, it, expect, beforeEach } from 'vitest'
import { createRouter, createMemoryHistory } from 'vue-router'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

describe('Router Guards', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  function createTestRouter(options: { authGuard?: boolean; guestGuard?: boolean } = {}) {
    const { authGuard = false, guestGuard = false } = options

    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', component: {} },
        {
          path: '/todo-lists',
          component: {},
          meta: authGuard ? { middleware: 'auth' } : {},
        },
        {
          path: '/auth/login',
          component: {},
          meta: guestGuard ? { middleware: 'guest' } : {},
        },
      ],
    })

    router.beforeEach((to) => {
      const auth = useAuthStore()

      if (to.meta.middleware === 'auth' && !auth.isAuthenticated) {
        return '/auth/login'
      }

      if (to.meta.middleware === 'guest' && auth.isAuthenticated) {
        return '/todo-lists'
      }
    })

    return router
  }

  it('redirects unauthenticated user to login for auth routes', async () => {
    const router = createTestRouter({ authGuard: true })
    const auth = useAuthStore()

    expect(auth.isAuthenticated).toBe(false)

    await router.push('/todo-lists')
    await router.isReady()

    expect(router.currentRoute.value.path).toBe('/auth/login')
  })

  it('allows authenticated user to access auth routes', async () => {
    const router = createTestRouter({ authGuard: true })
    const auth = useAuthStore()
    auth.accessToken = 'token'

    await router.push('/todo-lists')
    await router.isReady()

    expect(router.currentRoute.value.path).toBe('/todo-lists')
  })

  it('redirects authenticated user from guest routes', async () => {
    const router = createTestRouter({ guestGuard: true })
    const auth = useAuthStore()
    auth.accessToken = 'token'

    await router.push('/auth/login')
    await router.isReady()

    expect(router.currentRoute.value.path).toBe('/todo-lists')
  })

  it('allows unauthenticated user to access guest routes', async () => {
    const router = createTestRouter({ guestGuard: true })
    const auth = useAuthStore()

    expect(auth.isAuthenticated).toBe(false)

    await router.push('/auth/login')
    await router.isReady()

    expect(router.currentRoute.value.path).toBe('/auth/login')
  })

  it('allows access to routes without middleware', async () => {
    const router = createTestRouter()

    await router.push('/')
    await router.isReady()

    expect(router.currentRoute.value.path).toBe('/')
  })
})
