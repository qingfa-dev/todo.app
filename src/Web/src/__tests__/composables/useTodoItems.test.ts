import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useTodoItems } from '@/composables/useTodoItems'
import { useAuthStore } from '@/stores/auth'

vi.mock('@/lib/api', () => ({
  apiFetch: vi.fn(),
}))

import { apiFetch } from '@/lib/api'

describe('useTodoItems composable', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('fetchItems', () => {
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

      const api = useTodoItems()
      await api.fetchItems({ listId: 'l1', page: 1 })

      expect(apiFetch).toHaveBeenCalledWith('/todo-items', {
        token: 'test-token',
        query: { listId: 'l1', page: 1 },
      })
    })
  })

  describe('fetchItem', () => {
    it('fetches single item by id', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      const mockItem = { id: '1', listId: 'l1', title: 'Task', note: null, priority: 0, done: false }
      vi.mocked(apiFetch).mockResolvedValue(mockItem)

      const api = useTodoItems()
      const result = await api.fetchItem('1')

      expect(apiFetch).toHaveBeenCalledWith('/todo-items/1', { token: 'token' })
      expect(result).toEqual(mockItem)
    })
  })

  describe('createItem', () => {
    it('creates item with POST method', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      const mockItem = { id: '2', listId: 'l1', title: 'New task', note: null, priority: 0, done: false }
      vi.mocked(apiFetch).mockResolvedValue(mockItem)

      const api = useTodoItems()
      const result = await api.createItem({ listId: 'l1', title: 'New task' })

      expect(apiFetch).toHaveBeenCalledWith('/todo-items', {
        method: 'POST',
        token: 'token',
        body: { listId: 'l1', title: 'New task' },
      })
      expect(result).toEqual(mockItem)
    })

    it('sends optional fields', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      vi.mocked(apiFetch).mockResolvedValue({})

      const api = useTodoItems()
      await api.createItem({ listId: 'l1', title: 'Task', note: 'Note', priority: 2, done: true })

      expect(apiFetch).toHaveBeenCalledWith('/todo-items', {
        method: 'POST',
        token: 'token',
        body: { listId: 'l1', title: 'Task', note: 'Note', priority: 2, done: true },
      })
    })
  })

  describe('updateItem', () => {
    it('updates item with PUT method', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      vi.mocked(apiFetch).mockResolvedValue({})

      const api = useTodoItems()
      await api.updateItem('1', { listId: 'l1', title: 'Updated', done: true })

      expect(apiFetch).toHaveBeenCalledWith('/todo-items/1', {
        method: 'PUT',
        token: 'token',
        body: { listId: 'l1', title: 'Updated', done: true },
      })
    })
  })

  describe('deleteItem', () => {
    it('deletes item with DELETE method', async () => {
      const auth = useAuthStore()
      auth.accessToken = 'token'

      vi.mocked(apiFetch).mockResolvedValue(undefined)

      const api = useTodoItems()
      await api.deleteItem('1')

      expect(apiFetch).toHaveBeenCalledWith('/todo-items/1', {
        method: 'DELETE',
        token: 'token',
      })
    })
  })
})
