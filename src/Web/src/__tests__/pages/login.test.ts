import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import { setActivePinia, createPinia } from 'pinia'
import LoginPage from '@/pages/auth/login.vue'

vi.mock('@/lib/api', () => ({
  apiFetch: vi.fn(),
}))

import { apiFetch } from '@/lib/api'

function createTestRouter(component = LoginPage) {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/', component: {} },
      { path: '/auth/login', component },
      { path: '/auth/register', component: {} },
      { path: '/todo-lists', component: {} },
    ],
  })
}

describe('Login Page', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('renders login form', () => {
    const router = createTestRouter()
    const wrapper = mount(LoginPage, {
      global: { plugins: [router] },
    })

    expect(wrapper.text()).toContain('Sign In')
    expect(wrapper.find('input[type="email"]').exists()).toBe(true)
    expect(wrapper.find('input[type="password"]').exists()).toBe(true)
  })

  it('renders register link', () => {
    const router = createTestRouter()
    const wrapper = mount(LoginPage, {
      global: { plugins: [router] },
    })

    expect(wrapper.text()).toContain("Don't have an account?")
    expect(wrapper.text()).toContain('Create one')
  })

  it('shows API error on login failure', async () => {
    vi.mocked(apiFetch).mockRejectedValue({
      data: { title: 'Invalid credentials', status: 401 },
      status: 401,
    })

    const router = createTestRouter()
    const wrapper = mount(LoginPage, {
      global: { plugins: [router] },
    })

    await wrapper.find('input[type="email"]').setValue('test@example.com')
    await wrapper.find('input[type="password"]').setValue('password123')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(wrapper.text()).toContain('Invalid credentials')
  })

  it('shows validation error for empty email', async () => {
    const router = createTestRouter()
    const wrapper = mount(LoginPage, {
      global: { plugins: [router] },
    })

    await wrapper.find('input[type="email"]').setValue('')
    await wrapper.find('input[type="email"]').trigger('blur')

    expect(wrapper.text()).toContain('Please enter a valid email address')
  })

  it('disables submit button while loading', async () => {
    let resolveLogin: any
    vi.mocked(apiFetch).mockImplementation(
      () => new Promise((resolve) => { resolveLogin = resolve }),
    )

    const router = createTestRouter()
    const wrapper = mount(LoginPage, {
      global: { plugins: [router] },
    })

    await wrapper.find('input[type="email"]').setValue('test@example.com')
    await wrapper.find('input[type="password"]').setValue('password')
    await wrapper.find('form').trigger('submit')
    await wrapper.vm.$nextTick()

    const button = wrapper.find('button[type="submit"]')
    expect(button.attributes('disabled')).toBeDefined()
    expect(button.text()).toContain('Signing in...')

    resolveLogin({
      accessToken: 'token',
      refreshToken: 'refresh',
      expiresIn: 3600,
      tokenType: 'Bearer',
    })
  })
})
