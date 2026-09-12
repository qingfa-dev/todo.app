import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import { setActivePinia, createPinia } from 'pinia'
import RegisterPage from '@/pages/auth/register.vue'

vi.mock('@/lib/api', () => ({
  apiFetch: vi.fn(),
}))

import { apiFetch } from '@/lib/api'

function createTestRouter(component = RegisterPage) {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/', component: {} },
      { path: '/auth/register', component },
      { path: '/auth/login', component: {} },
      { path: '/todo-lists', component: {} },
    ],
  })
}

describe('Register Page', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('renders registration form', () => {
    const router = createTestRouter()
    const wrapper = mount(RegisterPage, {
      global: { plugins: [router] },
    })

    expect(wrapper.text()).toContain('Create Account')
    expect(wrapper.find('input[type="email"]').exists()).toBe(true)
    expect(wrapper.find('input[type="password"]').exists()).toBe(true)
  })

  it('renders login link', () => {
    const router = createTestRouter()
    const wrapper = mount(RegisterPage, {
      global: { plugins: [router] },
    })

    expect(wrapper.text()).toContain('Already have an account?')
    expect(wrapper.text()).toContain('Sign in')
  })

  it('shows validation error for short password', async () => {
    const router = createTestRouter()
    const wrapper = mount(RegisterPage, {
      global: { plugins: [router] },
    })

    await wrapper.find('input[type="password"]').setValue('short')
    await wrapper.find('input[type="password"]').trigger('blur')

    expect(wrapper.text()).toContain('Password must be at least 8 characters')
  })

  it('shows API error on registration failure', async () => {
    vi.mocked(apiFetch).mockRejectedValue({
      data: { title: 'Email already exists', status: 409 },
      status: 409,
    })

    const router = createTestRouter()
    const wrapper = mount(RegisterPage, {
      global: { plugins: [router] },
    })

    await wrapper.find('input[type="email"]').setValue('existing@example.com')
    await wrapper.find('input[type="password"]').setValue('password123')
    await wrapper.find('form').trigger('submit')
    await flushPromises()

    expect(wrapper.text()).toContain('Email already exists')
  })

  it('calls register on form submit', async () => {
    vi.mocked(apiFetch).mockResolvedValue({
      accessToken: 'token',
      refreshToken: 'refresh',
      expiresIn: 3600,
      tokenType: 'Bearer',
    })

    const router = createTestRouter()
    const wrapper = mount(RegisterPage, {
      global: { plugins: [router] },
    })

    await wrapper.find('input[type="email"]').setValue('new@example.com')
    await wrapper.find('input[type="password"]').setValue('password123')
    await wrapper.find('form').trigger('submit')

    expect(apiFetch).toHaveBeenCalledWith('/auth/register', {
      method: 'POST',
      body: { email: 'new@example.com', password: 'password123' },
    })
  })
})
