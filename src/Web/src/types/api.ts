export interface Error {
  code: string
  description: string
  statusCodeValue: number
}

export interface Result<T> {
  isSuccess: boolean
  value?: T
  errors?: Error[]
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

export interface PagedQuery {
  page?: number
  pageSize?: number
  sortBy?: string
  sortDirection?: SortDirection
}

export enum SortDirection {
  Ascending = 0,
  Descending = 1,
}
