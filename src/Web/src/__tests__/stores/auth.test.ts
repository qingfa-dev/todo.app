import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useAuthStore } from '@/stores/auth'

vi.mock('@/lib/api', () => ({
  apiFetch: vi.fn(),
}))

import { apiFetch } from '@/lib/api'

describe('Auth Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('initial state', () => {
    it('has null tokens and user', () => {
      const store = useAuthStore()
      expect(store.accessToken).toBeNull()
      expect(store.refreshToken).toBeNull()
      expect(store.user).toBeNull()
    })

    it('is not authenticated initially', () => {
      const store = useAuthStore()
      expect(store.isAuthenticated).toBe(false)
    })
  })

  describe('login', () => {
    it('sets tokens and user on successful login', async () => {
      const mockResponse = {
        accessToken: 'access-token-123',
        refreshToken: 'refresh-token-123',
        expiresIn: 3600,
        tokenType: 'Bearer',
      }
      vi.mocked(apiFetch).mockResolvedValue(mockResponse)

      const store = useAuthStore()
      await store.login({ email: 'test@example.com', password: 'password123' })

      expect(store.accessToken).toBe('access-token-123')
      expect(store.refreshToken).toBe('refresh-token-123')
      expect(store.user).toEqual({ email: 'test@example.com' })
      expect(store.isAuthenticated).toBe(true)
    })

    it('calls apiFetch with correct parameters', async () => {
      vi.mocked(apiFetch).mockResolvedValue({
        accessToken: 'token',
        refreshToken: 'refresh',
        expiresIn: 3600,
        tokenType: 'Bearer',
      })

      const store = useAuthStore()
      await store.login({ email: 'test@example.com', password: 'pass' })

      expect(apiFetch).toHaveBeenCalledWith('/auth/login', {
        method: 'POST',
        body: { email: 'test@example.com', password: 'pass' },
      })
    })

    it('throws on api error', async () => {
      vi.mocked(apiFetch).mockRejectedValue(new Error('Invalid credentials'))

      const store = useAuthStore()
      await expect(
        store.login({ email: 'test@example.com', password: 'wrong' }),
      ).rejects.toThrow('Invalid credentials')
    })
  })

  describe('register', () => {
    it('calls register then logs in', async () => {
      const mockResponse = {
        accessToken: 'new-access',
        refreshToken: 'new-refresh',
        expiresIn: 3600,
        tokenType: 'Bearer',
      }
      vi.mocked(apiFetch).mockResolvedValue(mockResponse)

      const store = useAuthStore()
      await store.register({ email: 'new@example.com', password: 'password123' })

      expect(apiFetch).toHaveBeenCalledWith('/auth/register', {
        method: 'POST',
        body: { email: 'new@example.com', password: 'password123' },
      })
      expect(apiFetch).toHaveBeenCalledWith('/auth/login', {
        method: 'POST',
        body: { email: 'new@example.com', password: 'password123' },
      })
      expect(store.isAuthenticated).toBe(true)
    })
  })

  describe('refresh', () => {
    it('updates tokens on successful refresh', async () => {
      const store = useAuthStore()
      store.refreshToken = 'old-refresh'

      vi.mocked(apiFetch).mockResolvedValue({
        accessToken: 'new-access',
        refreshToken: 'new-refresh',
        expiresIn: 3600,
        tokenType: 'Bearer',
      })

      await store.refresh()

      expect(store.accessToken).toBe('new-access')
      expect(store.refreshToken).toBe('new-refresh')
    })

    it('does nothing if no refresh token', async () => {
      const store = useAuthStore()
      await store.refresh()
      expect(apiFetch).not.toHaveBeenCalled()
    })

    it('logs out on refresh failure', async () => {
      const store = useAuthStore()
      store.refreshToken = 'expired-refresh'
      store.accessToken = 'old-access'

      vi.mocked(apiFetch).mockRejectedValue(new Error('Token expired'))

      await store.refresh()

      expect(store.accessToken).toBeNull()
      expect(store.refreshToken).toBeNull()
      expect(store.user).toBeNull()
    })
  })

  describe('logout', () => {
    it('clears all state', async () => {
      const store = useAuthStore()
      store.accessToken = 'token'
      store.refreshToken = 'refresh'
      store.user = { email: 'test@example.com' }

      vi.mocked(apiFetch).mockResolvedValue(undefined)

      await store.logout()

      expect(store.accessToken).toBeNull()
      expect(store.refreshToken).toBeNull()
      expect(store.user).toBeNull()
      expect(store.isAuthenticated).toBe(false)
    })

    it('calls logout endpoint with refresh token', async () => {
      const store = useAuthStore()
      store.accessToken = 'token'
      store.refreshToken = 'refresh'

      vi.mocked(apiFetch).mockResolvedValue(undefined)

      await store.logout()

      expect(apiFetch).toHaveBeenCalledWith('/auth/logout', {
        method: 'POST',
        token: 'token',
        body: { refreshToken: 'refresh' },
      })
    })

    it('clears state even if logout API fails', async () => {
      const store = useAuthStore()
      store.accessToken = 'token'
      store.refreshToken = 'refresh'
      store.user = { email: 'test@example.com' }

      vi.mocked(apiFetch).mockRejectedValue(new Error('Network error'))

      await store.logout()

      expect(store.accessToken).toBeNull()
      expect(store.refreshToken).toBeNull()
      expect(store.user).toBeNull()
    })

    it('does not call API if no refresh token', async () => {
      const store = useAuthStore()
      store.accessToken = 'token'

      await store.logout()

      expect(apiFetch).not.toHaveBeenCalled()
      expect(store.accessToken).toBeNull()
    })
  })
})
