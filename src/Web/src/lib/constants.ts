export const API_ENDPOINTS = {
  auth: {
    register: '/auth/register',
    login: '/auth/login',
    refresh: '/auth/refresh',
    logout: '/auth/logout',
  },
  todoLists: {
    base: '/todo-lists',
    byId: (id: string) => `/todo-lists/${id}`,
  },
  todoItems: {
    base: '/todo-items',
    byId: (id: string) => `/todo-items/${id}`,
  },
} as const
