export async function apiFetch<T>(
  path: string,
  options: {
    method?: string
    body?: unknown
    token?: string
    query?: Record<string, string | number | undefined | null>
  } = {},
): Promise<T> {
  const apiBase = import.meta.env.VITE_API_BASE ?? '/api'

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  }

  if (options.token) {
    headers.Authorization = `Bearer ${options.token}`
  }

  const queryEntries = Object.entries(options.query ?? {}).filter(
    ([, v]) => v != null && v !== '',
  )

  const queryString = queryEntries.length
    ? '?' +
      new URLSearchParams(
        queryEntries.map(([k, v]) => [k, String(v)]),
      ).toString()
    : ''

  const url = `${apiBase}${path}${queryString}`

  const response = await fetch(url, {
    method: options.method ?? 'GET',
    headers,
    body: options.body ? JSON.stringify(options.body) : undefined,
  })

  if (!response.ok) {
    const errorBody = await response.json().catch(() => null)
    throw { data: errorBody, status: response.status }
  }

  if (response.status === 204) {
    return undefined as T
  }

  return response.json()
}
