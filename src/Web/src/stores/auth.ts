import { defineStore } from 'pinia'
import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types/auth'
import { apiFetch } from '@/lib/api'
import { API_ENDPOINTS } from '@/lib/constants'

interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  user: { email: string } | null
}

const STORAGE_KEY = 'todo-app-auth'

function loadFromStorage(): AuthState | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    if (!raw) return null
    return JSON.parse(raw)
  } catch {
    return null
  }
}

function saveToStorage(state: AuthState) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state))
}

function clearStorage() {
  localStorage.removeItem(STORAGE_KEY)
}

const saved = loadFromStorage()

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    accessToken: saved?.accessToken ?? null,
    refreshToken: saved?.refreshToken ?? null,
    user: saved?.user ?? null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.accessToken,
  },

  actions: {
    async login(data: LoginRequest) {
      const response = await apiFetch<AuthResponse>(API_ENDPOINTS.auth.login, {
        method: 'POST',
        body: data,
      })

      this.accessToken = response.accessToken
      this.refreshToken = response.refreshToken
      this.user = { email: data.email }

      saveToStorage({
        accessToken: this.accessToken,
        refreshToken: this.refreshToken,
        user: this.user,
      })
    },

    async register(data: RegisterRequest) {
      await apiFetch(API_ENDPOINTS.auth.register, {
        method: 'POST',
        body: data,
      })

      await this.login({ email: data.email, password: data.password })
    },

    async refresh() {
      if (!this.refreshToken) return

      try {
        const response = await apiFetch<AuthResponse>(API_ENDPOINTS.auth.refresh, {
          method: 'POST',
          body: { refreshToken: this.refreshToken },
        })

        this.accessToken = response.accessToken
        this.refreshToken = response.refreshToken

        saveToStorage({
          accessToken: this.accessToken,
          refreshToken: this.refreshToken,
          user: this.user,
        })
      } catch {
        this.logout()
      }
    },

    async logout() {
      if (this.refreshToken) {
        try {
          await apiFetch(API_ENDPOINTS.auth.logout, {
            method: 'POST',
            token: this.accessToken ?? undefined,
            body: { refreshToken: this.refreshToken },
          })
        } catch {
          // ignore logout errors
        }
      }

      this.accessToken = null
      this.refreshToken = null
      this.user = null

      clearStorage()
    },
  },
})
