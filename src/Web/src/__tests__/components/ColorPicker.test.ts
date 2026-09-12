import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ColorPicker from '@/components/common/ColorPicker.vue'
import { SUPPORTED_COLOURS } from '@/types/todoList'

describe('ColorPicker', () => {
  it('renders all colour buttons', () => {
    const wrapper = mount(ColorPicker, {
      props: { modelValue: '#E05C4D' },
    })

    const buttons = wrapper.findAll('button')
    expect(buttons).toHaveLength(SUPPORTED_COLOURS.length)
  })

  it('emits update:modelValue when colour selected', async () => {
    const wrapper = mount(ColorPicker, {
      props: { modelValue: '#E05C4D' },
    })

    const buttons = wrapper.findAll('button')
    await buttons[2].trigger('click')

    expect(wrapper.emitted('update:modelValue')).toHaveLength(1)
    expect(wrapper.emitted('update:modelValue')![0]).toEqual([SUPPORTED_COLOURS[2].code])
  })

  it('applies active class to selected colour', () => {
    const wrapper = mount(ColorPicker, {
      props: { modelValue: '#E05C4D' },
    })

    const buttons = wrapper.findAll('button')
    const activeButton = buttons[0]
    expect(activeButton.classes()).toContain('border-gray-900')
    expect(activeButton.classes()).toContain('scale-110')
  })

  it('applies transparent border to unselected colours', () => {
    const wrapper = mount(ColorPicker, {
      props: { modelValue: '#E05C4D' },
    })

    const buttons = wrapper.findAll('button')
    const inactiveButton = buttons[1]
    expect(inactiveButton.classes()).toContain('border-transparent')
  })

  it('sets title attribute with colour name', () => {
    const wrapper = mount(ColorPicker, {
      props: { modelValue: '#E05C4D' },
    })

    const buttons = wrapper.findAll('button')
    expect(buttons[0].attributes('title')).toBe('Red')
  })
})
