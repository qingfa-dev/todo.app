import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useTodoItemsStore } from '@/stores/todoItems'

vi.mock('@/composables/useTodoItems', () => ({
  useTodoItems: vi.fn(),
}))

import { useTodoItems } from '@/composables/useTodoItems'

const mockApi = {
  fetchItems: vi.fn(),
  fetchItem: vi.fn(),
  createItem: vi.fn(),
  updateItem: vi.fn(),
  deleteItem: vi.fn(),
}

describe('TodoItems Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    vi.mocked(useTodoItems).mockReturnValue(mockApi as any)
  })

  describe('initial state', () => {
    it('has empty items and default pagination', () => {
      const store = useTodoItemsStore()
      expect(store.items).toEqual([])
      expect(store.page).toBe(1)
      expect(store.pageSize).toBe(20)
      expect(store.totalCount).toBe(0)
      expect(store.totalPages).toBe(0)
      expect(store.loading).toBe(false)
      expect(store.error).toBeNull()
    })
  })

  describe('fetchItems', () => {
    it('fetches and sets items', async () => {
      const mockResult = {
        items: [{ id: '1', listId: 'l1', title: 'Task', note: null, priority: 0, done: false }],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      }
      mockApi.fetchItems.mockResolvedValue(mockResult)

      const store = useTodoItemsStore()
      await store.fetchItems({ listId: 'l1' })

      expect(store.items).toEqual(mockResult.items)
      expect(store.loading).toBe(false)
    })

    it('sets error on failure', async () => {
      mockApi.fetchItems.mockRejectedValue({
        data: { title: 'Not found' },
      })

      const store = useTodoItemsStore()
      await store.fetchItems({ listId: 'l1' })

      expect(store.error).toBe('Not found')
      expect(store.loading).toBe(false)
    })
  })

  describe('createItem', () => {
    it('creates item then refetches', async () => {
      mockApi.createItem.mockResolvedValue({ id: '2' })
      mockApi.fetchItems.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoItemsStore()
      await store.createItem({ listId: 'l1', title: 'New task' })

      expect(mockApi.createItem).toHaveBeenCalledWith({ listId: 'l1', title: 'New task' })
      expect(mockApi.fetchItems).toHaveBeenCalled()
    })
  })

  describe('updateItem', () => {
    it('updates item then refetches', async () => {
      mockApi.updateItem.mockResolvedValue({ id: '1' })
      mockApi.fetchItems.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoItemsStore()
      await store.updateItem('1', { listId: 'l1', title: 'Updated', done: true })

      expect(mockApi.updateItem).toHaveBeenCalledWith('1', { listId: 'l1', title: 'Updated', done: true })
      expect(mockApi.fetchItems).toHaveBeenCalled()
    })
  })

  describe('toggleDone', () => {
    it('toggles done status', async () => {
      mockApi.updateItem.mockResolvedValue({ id: '1' })
      mockApi.fetchItems.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoItemsStore()
      const item = { id: '1', listId: 'l1', title: 'Task', note: null, priority: 0, done: false }
      await store.toggleDone('1', item)

      expect(mockApi.updateItem).toHaveBeenCalledWith('1', {
        listId: 'l1',
        title: 'Task',
        priority: 0,
        done: true,
      })
    })

    it('sets done to false when currently true', async () => {
      mockApi.updateItem.mockResolvedValue({ id: '1' })
      mockApi.fetchItems.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoItemsStore()
      const item = { id: '1', listId: 'l1', title: 'Task', note: null, priority: 0, done: true }
      await store.toggleDone('1', item)

      expect(mockApi.updateItem).toHaveBeenCalledWith('1', {
        listId: 'l1',
        title: 'Task',
        priority: 0,
        done: false,
      })
    })
  })

  describe('deleteItem', () => {
    it('deletes item then refetches', async () => {
      mockApi.deleteItem.mockResolvedValue(undefined)
      mockApi.fetchItems.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoItemsStore()
      await store.deleteItem('1')

      expect(mockApi.deleteItem).toHaveBeenCalledWith('1')
      expect(mockApi.fetchItems).toHaveBeenCalled()
    })
  })
})
