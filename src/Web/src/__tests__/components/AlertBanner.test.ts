import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import AlertBanner from '@/components/common/AlertBanner.vue'

describe('AlertBanner', () => {
  it('renders error message', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Something went wrong', severity: 'error' },
    })

    expect(wrapper.text()).toContain('Something went wrong')
  })

  it('renders warning message', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Duplicate entry', severity: 'warning' },
    })

    expect(wrapper.text()).toContain('Duplicate entry')
  })

  it('renders info message', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Not found', severity: 'info' },
    })

    expect(wrapper.text()).toContain('Not found')
  })

  it('applies error styling', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Error', severity: 'error' },
    })

    expect(wrapper.classes()).toContain('bg-red-50')
    expect(wrapper.classes()).toContain('border-red-200')
  })

  it('applies warning styling', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Warning', severity: 'warning' },
    })

    expect(wrapper.classes()).toContain('bg-amber-50')
    expect(wrapper.classes()).toContain('border-amber-200')
  })

  it('applies info styling', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Info', severity: 'info' },
    })

    expect(wrapper.classes()).toContain('bg-blue-50')
    expect(wrapper.classes()).toContain('border-blue-200')
  })

  it('shows dismiss button when dismissible', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Error', severity: 'error', dismissible: true },
    })

    expect(wrapper.find('button').exists()).toBe(true)
  })

  it('hides dismiss button when not dismissible', () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Error', severity: 'error', dismissible: false },
    })

    expect(wrapper.find('button').exists()).toBe(false)
  })

  it('emits dismiss event when dismiss button clicked', async () => {
    const wrapper = mount(AlertBanner, {
      props: { message: 'Error', severity: 'error', dismissible: true },
    })

    await wrapper.find('button').trigger('click')

    expect(wrapper.emitted('dismiss')).toHaveLength(1)
  })

  it('renders error detail when provided', () => {
    const wrapper = mount(AlertBanner, {
      props: {
        message: 'Duplicate entry',
        severity: 'warning',
        error: { title: 'TodoItem.Title.Duplicate', status: 409 },
      },
    })

    expect(wrapper.text()).toContain('TodoItem.Title.Duplicate')
  })
})
