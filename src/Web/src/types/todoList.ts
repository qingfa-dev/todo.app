import type { PagedQuery } from './api'

export interface TodoList {
  id: string
  title: string | null
  colour: string
}

export interface TodoListQuery extends PagedQuery {
  search?: string
  title?: string
  colour?: string
}

export const SUPPORTED_COLOURS = [
  { name: 'Red', code: '#E05C4D' },
  { name: 'Orange', code: '#D98B2B' },
  { name: 'Green', code: '#4CAF50' },
  { name: 'Teal', code: '#26A69A' },
  { name: 'Blue', code: '#5C6BC0' },
  { name: 'Purple', code: '#AB47BC' },
  { name: 'Grey', code: '#78909C' },
] as const
