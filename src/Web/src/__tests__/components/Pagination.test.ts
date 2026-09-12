import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import Pagination from '@/components/common/Pagination.vue'

describe('Pagination', () => {
  const defaultProps = {
    page: 1,
    totalPages: 5,
    hasPrevious: false,
    hasNext: true,
  }

  it('renders page info', () => {
    const wrapper = mount(Pagination, { props: defaultProps })
    expect(wrapper.text()).toContain('Page 1 of 5')
  })

  it('emits pageChange with previous page', async () => {
    const wrapper = mount(Pagination, {
      props: { ...defaultProps, page: 3, hasPrevious: true, hasNext: true },
    })

    await wrapper.findAll('button')[0].trigger('click')

    expect(wrapper.emitted('pageChange')).toHaveLength(1)
    expect(wrapper.emitted('pageChange')![0]).toEqual([2])
  })

  it('emits pageChange with next page', async () => {
    const wrapper = mount(Pagination, { props: defaultProps })

    await wrapper.findAll('button')[1].trigger('click')

    expect(wrapper.emitted('pageChange')).toHaveLength(1)
    expect(wrapper.emitted('pageChange')![0]).toEqual([2])
  })

  it('disables previous button when hasPrevious is false', () => {
    const wrapper = mount(Pagination, { props: defaultProps })
    const prevButton = wrapper.findAll('button')[0]
    expect(prevButton.attributes('disabled')).toBeDefined()
  })

  it('disables next button when hasNext is false', () => {
    const wrapper = mount(Pagination, {
      props: { ...defaultProps, hasNext: false },
    })
    const nextButton = wrapper.findAll('button')[1]
    expect(nextButton.attributes('disabled')).toBeDefined()
  })

  it('enables both buttons when both navigation options available', () => {
    const wrapper = mount(Pagination, {
      props: { ...defaultProps, page: 3, hasPrevious: true, hasNext: true },
    })
    const buttons = wrapper.findAll('button')
    expect(buttons[0].attributes('disabled')).toBeUndefined()
    expect(buttons[1].attributes('disabled')).toBeUndefined()
  })
})
