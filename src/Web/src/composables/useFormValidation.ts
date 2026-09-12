import { reactive, computed } from 'vue'

export interface ValidationRule {
  required?: boolean
  minLength?: number
  maxLength?: number
  pattern?: RegExp
  message?: string
}

export type ValidationRules<T> = {
  [K in keyof T]?: ValidationRule
}

export type ValidationErrors<T> = {
  [K in keyof T]?: string
}

export function useFormValidation<T extends Record<string, any>>(
  form: T,
  rules: ValidationRules<T>,
) {
  const errors = reactive<ValidationErrors<T>>({})
  const touched = reactive<Record<keyof T, boolean>>({} as Record<keyof T, boolean>)

  function validate(field: keyof T): boolean {
    touched[field] = true
    const rule = rules[field]
    if (!rule) {
      errors[field] = undefined
      return true
    }

    const value = form[field]

    if (rule.required && (value === '' || value === null || value === undefined)) {
      errors[field] = rule.message ?? 'This field is required'
      return false
    }

    if (rule.minLength && typeof value === 'string' && value.length < rule.minLength) {
      errors[field] = rule.message ?? `Minimum ${rule.minLength} characters`
      return false
    }

    if (rule.maxLength && typeof value === 'string' && value.length > rule.maxLength) {
      errors[field] = rule.message ?? `Maximum ${rule.maxLength} characters`
      return false
    }

    if (rule.pattern && typeof value === 'string' && !rule.pattern.test(value)) {
      errors[field] = rule.message ?? 'Invalid format'
      return false
    }

    errors[field] = undefined
    return true
  }

  function validateAll(): boolean {
    let valid = true
    for (const field of Object.keys(rules) as (keyof T)[]) {
      if (!validate(field)) {
        valid = false
      }
    }
    return valid
  }

  function touch(field: keyof T) {
    touched[field] = true
  }

  function reset() {
    for (const field of Object.keys(errors) as (keyof T)[]) {
      errors[field] = undefined
    }
    for (const field of Object.keys(touched) as (keyof T)[]) {
      touched[field] = false
    }
  }

  const isValid = computed(() => {
    return Object.values(errors).every((e) => !e)
  })

  function hasError(field: keyof T): boolean {
    return touched[field] === true && !!errors[field]
  }

  return { errors, touched, validate, validateAll, touch, reset, isValid, hasError }
}
