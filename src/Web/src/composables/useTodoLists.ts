import type { PagedResult } from '@/types/api'
import type { TodoList, TodoListQuery } from '@/types/todoList'
import { useAuthStore } from '@/stores/auth'
import { API_ENDPOINTS } from '@/lib/constants'
import { apiFetch } from '@/lib/api'

export function useTodoLists() {
  const auth = useAuthStore()

  async function fetchLists(query: TodoListQuery = {}): Promise<PagedResult<TodoList>> {
    return apiFetch<PagedResult<TodoList>>(API_ENDPOINTS.todoLists.base, {
      token: auth.accessToken ?? undefined,
      query: query as Record<string, string | number | undefined | null>,
    })
  }

  async function fetchList(id: string): Promise<TodoList> {
    return apiFetch<TodoList>(API_ENDPOINTS.todoLists.byId(id), {
      token: auth.accessToken ?? undefined,
    })
  }

  async function createList(data: { title: string; colour: string }): Promise<TodoList> {
    return apiFetch<TodoList>(API_ENDPOINTS.todoLists.base, {
      method: 'POST',
      token: auth.accessToken ?? undefined,
      body: data,
    })
  }

  async function updateList(
    id: string,
    data: { title: string; colour: string },
  ): Promise<TodoList> {
    return apiFetch<TodoList>(API_ENDPOINTS.todoLists.byId(id), {
      method: 'PUT',
      token: auth.accessToken ?? undefined,
      body: data,
    })
  }

  async function deleteList(id: string): Promise<void> {
    await apiFetch(API_ENDPOINTS.todoLists.byId(id), {
      method: 'DELETE',
      token: auth.accessToken ?? undefined,
    })
  }

  return { fetchLists, fetchList, createList, updateList, deleteList }
}
