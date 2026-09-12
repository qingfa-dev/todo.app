import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import TodoListForm from '@/components/todo-lists/TodoListForm.vue'

describe('TodoListForm', () => {
  const globalStubs = {
    BaseModal: {
      template: '<div class="modal-stub"><slot /></div>',
      props: ['title'],
    },
  }

  it('renders form fields', () => {
    const wrapper = mount(TodoListForm, { global: { stubs: globalStubs } })

    expect(wrapper.find('input[type="text"]').exists()).toBe(true)
    expect(wrapper.text()).toContain('Title')
    expect(wrapper.text()).toContain('Colour')
  })

  it('renders save and cancel buttons', () => {
    const wrapper = mount(TodoListForm, { global: { stubs: globalStubs } })

    expect(wrapper.text()).toContain('Save')
    expect(wrapper.text()).toContain('Cancel')
  })

  it('emits cancel when cancel button clicked', async () => {
    const wrapper = mount(TodoListForm, { global: { stubs: globalStubs } })

    const cancelButton = wrapper.findAll('button').find((b) => b.text() === 'Cancel')
    expect(cancelButton).toBeDefined()
    await cancelButton!.trigger('click')

    expect(wrapper.emitted('cancel')).toHaveLength(1)
  })

  it('shows validation error for empty title', async () => {
    const wrapper = mount(TodoListForm, { global: { stubs: globalStubs } })

    await wrapper.find('input[type="text"]').setValue('')
    await wrapper.find('input[type="text"]').trigger('blur')

    expect(wrapper.text()).toContain('Title is required')
  })

  it('emits submit with form data', async () => {
    const wrapper = mount(TodoListForm, { global: { stubs: globalStubs } })

    await wrapper.find('input[type="text"]').setValue('New List')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('submit')).toHaveLength(1)
    expect(wrapper.emitted('submit')![0]).toEqual([
      { title: 'New List', colour: '#78909C' },
    ])
  })
})
