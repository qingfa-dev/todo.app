import { defineStore } from 'pinia'
import type { TodoItem, TodoItemQuery } from '@/types/todoItem'
import { useTodoItems } from '@/composables/useTodoItems'

interface TodoItemsState {
  items: TodoItem[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
  loading: boolean
  error: string | null
  query: TodoItemQuery
}

export const useTodoItemsStore = defineStore('todoItems', {
  state: (): TodoItemsState => ({
    items: [],
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
    loading: false,
    error: null,
    query: {},
  }),

  actions: {
    async fetchItems(query: TodoItemQuery = {}) {
      this.loading = true
      this.error = null

      try {
        this.query = { ...this.query, ...query }
        const api = useTodoItems()
        const result = await api.fetchItems(this.query)

        this.items = result.items
        this.page = result.page
        this.pageSize = result.pageSize
        this.totalCount = result.totalCount
        this.totalPages = result.totalPages
        this.hasPreviousPage = result.hasPreviousPage
        this.hasNextPage = result.hasNextPage
      } catch (e: any) {
        this.error = e.data?.title ?? e.message ?? 'Failed to fetch items'
      } finally {
        this.loading = false
      }
    },

    async createItem(data: {
      listId: string
      title: string
      note?: string
      priority?: number
      done?: boolean
    }) {
      const api = useTodoItems()
      await api.createItem(data)
      await this.refresh()
    },

    async updateItem(
      id: string,
      data: {
        listId: string
        title: string
        note?: string
        priority?: number
        done?: boolean
      },
    ) {
      const api = useTodoItems()
      await api.updateItem(id, data)
      await this.refresh()
    },

    async toggleDone(id: string, item: TodoItem) {
      const api = useTodoItems()
      await api.updateItem(id, {
        listId: item.listId,
        title: item.title ?? '',
        note: item.note ?? undefined,
        priority: item.priority,
        done: !item.done,
      })
      await this.refresh()
    },

    async deleteItem(id: string) {
      const api = useTodoItems()
      await api.deleteItem(id)
      await this.refresh()
    },

    async refresh() {
      const api = useTodoItems()
      const result = await api.fetchItems(this.query)

      this.items = result.items
      this.page = result.page
      this.pageSize = result.pageSize
      this.totalCount = result.totalCount
      this.totalPages = result.totalPages
      this.hasPreviousPage = result.hasPreviousPage
      this.hasNextPage = result.hasNextPage
    },

    reset() {
      this.items = []
      this.page = 1
      this.pageSize = 20
      this.totalCount = 0
      this.totalPages = 0
      this.hasPreviousPage = false
      this.hasNextPage = false
      this.loading = false
      this.error = null
      this.query = {}
    },
  },
})
