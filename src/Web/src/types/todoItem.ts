import type { PagedQuery } from './api'

export enum PriorityLevel {
  None = 0,
  Low = 1,
  Medium = 2,
  High = 3,
}

export interface TodoItem {
  id: string
  listId: string
  title: string | null
  note: string | null
  priority: PriorityLevel
  done: boolean
}

export interface TodoItemQuery extends PagedQuery {
  search?: string
  title?: string
  note?: string
  priority?: PriorityLevel
  done?: boolean
  listId?: string
}

export const PRIORITY_LABELS: Record<PriorityLevel, string> = {
  [PriorityLevel.None]: 'None',
  [PriorityLevel.Low]: 'Low',
  [PriorityLevel.Medium]: 'Medium',
  [PriorityLevel.High]: 'High',
}

export const PRIORITY_COLORS: Record<PriorityLevel, string> = {
  [PriorityLevel.None]: 'bg-gray-100 text-gray-700',
  [PriorityLevel.Low]: 'bg-blue-100 text-blue-700',
  [PriorityLevel.Medium]: 'bg-yellow-100 text-yellow-700',
  [PriorityLevel.High]: 'bg-red-100 text-red-700',
}
