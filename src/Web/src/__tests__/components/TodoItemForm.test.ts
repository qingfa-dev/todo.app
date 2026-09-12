import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import TodoItemForm from '@/components/todo-items/TodoItemForm.vue'

describe('TodoItemForm', () => {
  const globalStubs = {
    BaseModal: {
      template: '<div class="modal-stub"><slot /></div>',
      props: ['title'],
    },
  }

  it('renders form fields', () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    expect(wrapper.find('input[type="text"]').exists()).toBe(true)
    expect(wrapper.find('textarea').exists()).toBe(true)
    expect(wrapper.find('select').exists()).toBe(true)
  })

  it('renders priority options', () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    const options = wrapper.findAll('option')
    expect(options).toHaveLength(4)
    expect(options[0].text()).toBe('None')
    expect(options[1].text()).toBe('Low')
    expect(options[2].text()).toBe('Medium')
    expect(options[3].text()).toBe('High')
  })

  it('renders Add and Cancel buttons', () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    expect(wrapper.text()).toContain('Add')
    expect(wrapper.text()).toContain('Cancel')
  })

  it('emits cancel when cancel button clicked', async () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    const cancelButton = wrapper.findAll('button').find((b) => b.text() === 'Cancel')
    expect(cancelButton).toBeDefined()
    await cancelButton!.trigger('click')

    expect(wrapper.emitted('cancel')).toHaveLength(1)
  })

  it('shows validation error for empty title', async () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    await wrapper.find('input[type="text"]').setValue('')
    await wrapper.find('input[type="text"]').trigger('blur')

    expect(wrapper.text()).toContain('Title is required')
  })

  it('emits submit with form data', async () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    await wrapper.find('input[type="text"]').setValue('Buy milk')
    await wrapper.find('textarea').setValue('2% milk')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('submit')).toHaveLength(1)
    const emitted = wrapper.emitted('submit')![0] as any[]
    expect(emitted[0].title).toBe('Buy milk')
    expect(emitted[0].note).toBe('2% milk')
  })

  it('sends undefined for empty note', async () => {
    const wrapper = mount(TodoItemForm, {
      props: { listId: 'l1' },
      global: { stubs: globalStubs },
    })

    await wrapper.find('input[type="text"]').setValue('Task')
    await wrapper.find('form').trigger('submit')

    expect(wrapper.emitted('submit')![0]).toEqual([
      { title: 'Task', note: undefined, priority: 0 },
    ])
  })
})
