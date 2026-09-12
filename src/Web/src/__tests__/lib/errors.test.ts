import { describe, it, expect } from 'vitest'
import {
  getSeverityForStatus,
  getSeverityClasses,
  parseApiError,
} from '@/lib/errors'

describe('Error Handling', () => {
  describe('getSeverityForStatus', () => {
    it('returns error for 500+', () => {
      expect(getSeverityForStatus(500)).toBe('error')
      expect(getSeverityForStatus(503)).toBe('error')
    })

    it('returns warning for 409 Conflict', () => {
      expect(getSeverityForStatus(409)).toBe('warning')
    })

    it('returns warning for 400 Bad Request', () => {
      expect(getSeverityForStatus(400)).toBe('warning')
    })

    it('returns warning for 422 Unprocessable Entity', () => {
      expect(getSeverityForStatus(422)).toBe('warning')
    })

    it('returns info for 404 Not Found', () => {
      expect(getSeverityForStatus(404)).toBe('info')
    })

    it('returns error for 401 Unauthorized', () => {
      expect(getSeverityForStatus(401)).toBe('error')
    })

    it('returns error for 403 Forbidden', () => {
      expect(getSeverityForStatus(403)).toBe('error')
    })
  })

  describe('getSeverityClasses', () => {
    it('returns red classes for error', () => {
      const classes = getSeverityClasses('error')
      expect(classes.bg).toBe('bg-red-50')
      expect(classes.border).toBe('border-red-200')
      expect(classes.text).toBe('text-red-700')
    })

    it('returns amber classes for warning', () => {
      const classes = getSeverityClasses('warning')
      expect(classes.bg).toBe('bg-amber-50')
      expect(classes.border).toBe('border-amber-200')
      expect(classes.text).toBe('text-amber-700')
    })

    it('returns blue classes for info', () => {
      const classes = getSeverityClasses('info')
      expect(classes.bg).toBe('bg-blue-50')
      expect(classes.border).toBe('border-blue-200')
      expect(classes.text).toBe('text-blue-700')
    })
  })

  describe('parseApiError', () => {
    it('parses API error with status', () => {
      const error = {
        data: { title: 'Duplicate', status: 409, detail: 'Already exists' },
        status: 409,
      }
      const result = parseApiError(error)

      expect(result.message).toBe('Already exists')
      expect(result.severity).toBe('warning')
      expect(result.error?.title).toBe('Duplicate')
    })

    it('parses API error with 409 status', () => {
      const error = {
        data: {
          type: 'https://tools.ietf.org/html/rfc9110#section-15.5.10',
          title: 'TodoItem.Title.Duplicate',
          status: 409,
          detail: 'A todo item with the same title already exists in this list.',
        },
        status: 409,
      }
      const result = parseApiError(error)

      expect(result.message).toBe('A todo item with the same title already exists in this list.')
      expect(result.severity).toBe('warning')
      expect(result.error?.title).toBe('TodoItem.Title.Duplicate')
    })

    it('parses API error with 500 status', () => {
      const error = {
        data: { title: 'Internal Server Error', status: 500 },
        status: 500,
      }
      const result = parseApiError(error)

      expect(result.severity).toBe('error')
    })

    it('parses API error with 404 status', () => {
      const error = {
        data: { title: 'Not Found', status: 404 },
        status: 404,
      }
      const result = parseApiError(error)

      expect(result.severity).toBe('info')
    })

    it('handles unknown error types', () => {
      const result = parseApiError('something went wrong')

      expect(result.message).toBe('An unexpected error occurred')
      expect(result.severity).toBe('error')
    })

    it('handles null error', () => {
      const result = parseApiError(null)

      expect(result.message).toBe('An unexpected error occurred')
      expect(result.severity).toBe('error')
    })
  })
})
