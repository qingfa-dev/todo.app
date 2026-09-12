import { describe, it, expect, beforeEach, vi } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useTodoListsStore } from '@/stores/todoLists'

vi.mock('@/composables/useTodoLists', () => ({
  useTodoLists: vi.fn(),
}))

import { useTodoLists } from '@/composables/useTodoLists'

const mockApi = {
  fetchLists: vi.fn(),
  fetchList: vi.fn(),
  createList: vi.fn(),
  updateList: vi.fn(),
  deleteList: vi.fn(),
}

describe('TodoLists Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
    vi.mocked(useTodoLists).mockReturnValue(mockApi as any)
  })

  describe('initial state', () => {
    it('has empty items and default pagination', () => {
      const store = useTodoListsStore()
      expect(store.items).toEqual([])
      expect(store.page).toBe(1)
      expect(store.pageSize).toBe(20)
      expect(store.totalCount).toBe(0)
      expect(store.totalPages).toBe(0)
      expect(store.hasPreviousPage).toBe(false)
      expect(store.hasNextPage).toBe(false)
      expect(store.loading).toBe(false)
      expect(store.error).toBeNull()
    })
  })

  describe('fetchLists', () => {
    it('fetches and sets items', async () => {
      const mockResult = {
        items: [{ id: '1', title: 'Test List', colour: '#E05C4D' }],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      }
      mockApi.fetchLists.mockResolvedValue(mockResult)

      const store = useTodoListsStore()
      await store.fetchLists()

      expect(store.items).toEqual(mockResult.items)
      expect(store.page).toBe(1)
      expect(store.totalCount).toBe(1)
      expect(store.loading).toBe(false)
      expect(store.error).toBeNull()
    })

    it('sets loading to true while fetching', async () => {
      let resolvePromise: any
      mockApi.fetchLists.mockImplementation(
        () => new Promise((resolve) => { resolvePromise = resolve }),
      )

      const store = useTodoListsStore()
      const fetchPromise = store.fetchLists()

      expect(store.loading).toBe(true)

      resolvePromise({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })
      await fetchPromise

      expect(store.loading).toBe(false)
    })

    it('sets error on failure', async () => {
      mockApi.fetchLists.mockRejectedValue({
        data: { title: 'Server error' },
      })

      const store = useTodoListsStore()
      await store.fetchLists()

      expect(store.error).toBe('Server error')
      expect(store.loading).toBe(false)
    })

    it('merges query parameters', async () => {
      mockApi.fetchLists.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoListsStore()
      await store.fetchLists({ search: 'test', page: 2 })

      expect(mockApi.fetchLists).toHaveBeenCalledWith({
        search: 'test',
        page: 2,
      })
    })
  })

  describe('createList', () => {
    it('creates list then refetches', async () => {
      mockApi.createList.mockResolvedValue({ id: '2', title: 'New', colour: '#4CAF50' })
      mockApi.fetchLists.mockResolvedValue({
        items: [{ id: '2', title: 'New', colour: '#4CAF50' }],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoListsStore()
      await store.createList({ title: 'New', colour: '#4CAF50' })

      expect(mockApi.createList).toHaveBeenCalledWith({ title: 'New', colour: '#4CAF50' })
      expect(mockApi.fetchLists).toHaveBeenCalled()
    })
  })

  describe('updateList', () => {
    it('updates list then refetches', async () => {
      mockApi.updateList.mockResolvedValue({ id: '1', title: 'Updated', colour: '#E05C4D' })
      mockApi.fetchLists.mockResolvedValue({
        items: [{ id: '1', title: 'Updated', colour: '#E05C4D' }],
        page: 1,
        pageSize: 20,
        totalCount: 1,
        totalPages: 1,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoListsStore()
      await store.updateList('1', { title: 'Updated', colour: '#E05C4D' })

      expect(mockApi.updateList).toHaveBeenCalledWith('1', { title: 'Updated', colour: '#E05C4D' })
      expect(mockApi.fetchLists).toHaveBeenCalled()
    })
  })

  describe('deleteList', () => {
    it('deletes list then refetches', async () => {
      mockApi.deleteList.mockResolvedValue(undefined)
      mockApi.fetchLists.mockResolvedValue({
        items: [],
        page: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false,
      })

      const store = useTodoListsStore()
      await store.deleteList('1')

      expect(mockApi.deleteList).toHaveBeenCalledWith('1')
      expect(mockApi.fetchLists).toHaveBeenCalled()
    })
  })
})
