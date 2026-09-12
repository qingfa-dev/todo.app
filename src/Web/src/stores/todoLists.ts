import { defineStore } from 'pinia'
import type { PagedResult } from '@/types/api'
import type { TodoList, TodoListQuery } from '@/types/todoList'
import { useTodoLists } from '@/composables/useTodoLists'

interface TodoListsState {
  items: TodoList[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
  loading: boolean
  error: string | null
  query: TodoListQuery
}

export const useTodoListsStore = defineStore('todoLists', {
  state: (): TodoListsState => ({
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
    async fetchLists(query: TodoListQuery = {}) {
      this.loading = true
      this.error = null

      try {
        this.query = { ...this.query, ...query }
        const api = useTodoLists()
        const result = await api.fetchLists(this.query)

        this.items = result.items
        this.page = result.page
        this.pageSize = result.pageSize
        this.totalCount = result.totalCount
        this.totalPages = result.totalPages
        this.hasPreviousPage = result.hasPreviousPage
        this.hasNextPage = result.hasNextPage
      } catch (e: any) {
        this.error = e.data?.title ?? e.message ?? 'Failed to fetch lists'
      } finally {
        this.loading = false
      }
    },

    async createList(data: { title: string; colour: string }) {
      const api = useTodoLists()
      await api.createList(data)
      await this.refresh()
    },

    async updateList(id: string, data: { title: string; colour: string }) {
      const api = useTodoLists()
      await api.updateList(id, data)
      await this.refresh()
    },

    async deleteList(id: string) {
      const api = useTodoLists()
      await api.deleteList(id)
      await this.refresh()
    },

    async refresh() {
      const api = useTodoLists()
      const result = await api.fetchLists(this.query)

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
