import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useTodoLists } from '@/composables/useTodoLists'
import { useAuthStore } from '@/stores/auth'

vi.mock('@/lib/api', () => ({
  apiFetch: vi.fn(),
}))

import { apiFetch } from '@/lib/api'

describe('useTodoLists composable', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('fetchLists', () => {
    it('calls apiFetch with correct parameters', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'test-token'

      vi.mocked(apiFetch).mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const api = useTodoLists()
      await api.fetchLists({ page: 2, search: 'test' })

      expect(apiFetch).toHaveBeenCalledWith('/todo-lists', {
        token: 'test-token',
        query: { page: 2, search: 'test' },
      })
    })

    it('returns paged result', async () => {
      const mockResult = {
        items: [{ id: '1', title: 'List', colour: '#E05C4D' }],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      }
      vi.mocked(apiFetch).mockResolvedValue(mockResult)

      const api = useTodoLists()
      const result = await api.fetchLists()

      expect(result).toEqual(mockResult)
    })
  })

  describe('fetchList', () => {
    it('fetches single list by id', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      const mockList = { id: '1', title: 'My List', colour: '#4CAF50' }
      vi.mocked(apiFetch).mockResolvedValue(mockList)

      const api = useTodoLists()
      const result = await api.fetchList('1')

      expect(apiFetch).toHaveBeenCalledWith('/todo-lists/1', { token: 'token' })
      expect(result).toEqual(mockList)
    })
  })

  describe('createList', () => {
    it('creates list with POST method', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      const mockList = { id: '2', title: 'New', colour: '#E05C4D' }
      vi.mocked(apiFetch).mockResolvedValue(mockList)

      const api = useTodoLists()
      const result = await api.createList({ title: 'New', colour: '#E05C4D' })

      expect(apiFetch).toHaveBeenCalledWith('/todo-lists', {
        method: 'POST',
        token: 'token',
        body: { title: 'New', colour: '#E05C4D' },
      })
      expect(result).toEqual(mockList)
    })
  })

  describe('updateList', () => {
    it('updates list with PUT method', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      vi.mocked(apiFetch).mockResolvedValue({ id: '1', title: 'Updated', colour: '#4CAF50' })

      const api = useTodoLists()
      await api.updateList('1', { title: 'Updated', colour: '#4CAF50' })

      expect(apiFetch).toHaveBeenCalledWith('/todo-lists/1', {
        method: 'PUT',
        token: 'token',
        body: { title: 'Updated', colour: '#4CAF50' },
      })
    })
  })

  describe('deleteList', () => {
    it('deletes list with DELETE method', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      vi.mocked(apiFetch).mockResolvedValue(undefined)

      const api = useTodoLists()
      await api.deleteList('1')

      expect(apiFetch).toHaveBeenCalledWith('/todo-lists/1', {
        method: 'DELETE',
        token: 'token',
      })
    })
  })
})
