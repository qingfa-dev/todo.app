import { reactive } from 'vue'
import type { ApiError, ApiErrorState } from '@/lib/errors'
import { parseApiError } from '@/lib/errors'

export interface Notification {
  id: string
  message: string
  error: ApiError | null
  severity: 'error' | 'warning' | 'info'
}

const notifications = reactive<Notification[]>([])

export function useNotifications() {
  function add(error: unknown): Notification {
    const parsed = parseApiError(error)
    const notification: Notification = {
      id: crypto.randomUUID(),
      message: parsed.message,
      error: parsed.error,
      severity: parsed.severity,
    }
    notifications.push(notification)
    return notification
  }

  function remove(id: string) {
    const index = notifications.findIndex((n) => n.id === id)
    if (index !== -1) {
      notifications.splice(index, 1)
    }
  }

  function clear() {
    notifications.splice(0, notifications.length)
  }

  function success(message: string) {
    const notification: Notification = {
      id: crypto.randomUUID(),
      message,
      error: null,
      severity: 'info',
    }
    notifications.push(notification)
    return notification
  }

  return { notifications, add, remove, clear, success }
}
