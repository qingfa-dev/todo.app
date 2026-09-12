export interface ApiError {
  type?: string
  title: string
  status: number
  detail?: string
  errors?: Record<string, string[]>
}

export interface ApiErrorState {
  error: ApiError | null
  message: string
  severity: 'error' | 'warning' | 'info'
}

export function getSeverityForStatus(status: number): 'error' | 'warning' | 'info' {
  if (status >= 500) return 'error'
  if (status === 409) return 'warning'
  if (status === 404) return 'info'
  if (status === 400 || status === 422) return 'warning'
  if (status === 401 || status === 403) return 'error'
  return 'error'
}

export function getSeverityClasses(severity: 'error' | 'warning' | 'info'): {
  bg: string
  border: string
  text: string
  icon: string
} {
  switch (severity) {
    case 'error':
      return {
        bg: 'bg-red-50',
        border: 'border-red-200',
        text: 'text-red-700',
        icon: 'text-red-500',
      }
    case 'warning':
      return {
        bg: 'bg-amber-50',
        border: 'border-amber-200',
        text: 'text-amber-700',
        icon: 'text-amber-500',
      }
    case 'info':
      return {
        bg: 'bg-blue-50',
        border: 'border-blue-200',
        text: 'text-blue-700',
        icon: 'text-blue-500',
      }
  }
}

export function parseApiError(error: unknown): ApiErrorState {
  if (!error || typeof error !== 'object') {
    return {
      error: null,
      message: 'An unexpected error occurred',
      severity: 'error',
    }
  }

  const apiError = error as { data?: ApiError; status?: number }

  if (apiError.data) {
    const status = apiError.status ?? apiError.data.status ?? 500
    return {
      error: apiError.data,
      message: apiError.data.detail ?? apiError.data.title ?? 'Request failed',
      severity: getSeverityForStatus(status),
    }
  }

  if (apiError.status) {
    return {
      error: null,
      message: `Request failed with status ${apiError.status}`,
      severity: getSeverityForStatus(apiError.status),
    }
  }

  return {
    error: null,
    message: (error as Error).message ?? 'An unexpected error occurred',
    severity: 'error',
  }
}
