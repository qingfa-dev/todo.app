import type { PagedResult } from '@/types/api'
import type { TodoItem, TodoItemQuery } from '@/types/todoItem'
import { useAuthStore } from '@/stores/auth'
import { API_ENDPOINTS } from '@/lib/constants'
import { apiFetch } from '@/lib/api'

export function useTodoItems() {
  const auth = useAuthStore()

  async function fetchItems(query: TodoItemQuery = {}): Promise<PagedResult<TodoItem>> {
    return apiFetch<PagedResult<TodoItem>>(API_ENDPOINTS.todoItems.base, {
      token: auth.accessToken ?? undefined,
      query: query as Record<string, string | number | undefined | null>,
    })
  }

  async function fetchItem(id: string): Promise<TodoItem> {
    return apiFetch<TodoItem>(API_ENDPOINTS.todoItems.byId(id), {
      token: auth.accessToken ?? undefined,
    })
  }

  async function createItem(data: {
    listId: string
    title: string
    note?: string
    priority?: number
    done?: boolean
  }): Promise<TodoItem> {
    return apiFetch<TodoItem>(API_ENDPOINTS.todoItems.base, {
      method: 'POST',
      token: auth.accessToken ?? undefined,
      body: data,
    })
  }

  async function updateItem(
    id: string,
    data: {
      listId: string
      title: string
      note?: string
      priority?: number
      done?: boolean
    },
  ): Promise<TodoItem> {
    return apiFetch<TodoItem>(API_ENDPOINTS.todoItems.byId(id), {
      method: 'PUT',
      token: auth.accessToken ?? undefined,
      body: data,
    })
  }

  async function deleteItem(id: string): Promise<void> {
    await apiFetch(API_ENDPOINTS.todoItems.byId(id), {
      method: 'DELETE',
      token: auth.accessToken ?? undefined,
    })
  }

  return { fetchItems, fetchItem, createItem, updateItem, deleteItem }
}
