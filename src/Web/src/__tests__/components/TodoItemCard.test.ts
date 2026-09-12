import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import TodoItemCard from '@/components/todo-items/TodoItemCard.vue'

describe('TodoItemCard', () => {
  const defaultItem = {
    id: 'item-1',
    listId: 'list-1',
    title: 'Buy groceries',
    note: 'Milk, eggs, bread',
    priority: 1,
    done: false,
  }

  it('renders item title', () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })
    expect(wrapper.text()).toContain('Buy groceries')
  })

  it('renders note when present', () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })
    expect(wrapper.text()).toContain('Milk, eggs, bread')
  })

  it('does not render note when null', () => {
    const item = { ...defaultItem, note: null }
    const wrapper = mount(TodoItemCard, { props: { item } })
    expect(wrapper.text()).not.toContain('Milk')
  })

  it('renders priority label', () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })
    expect(wrapper.text()).toContain('Low')
  })

  it('renders correct priority for different levels', () => {
    const item = { ...defaultItem, priority: 3 }
    const wrapper = mount(TodoItemCard, { props: { item } })
    expect(wrapper.text()).toContain('High')
  })

  it('renders checkbox unchecked when done is false', () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })
    const checkbox = wrapper.find('input[type="checkbox"]')
    expect(checkbox.element.checked).toBe(false)
  })

  it('renders checkbox checked when done is true', () => {
    const item = { ...defaultItem, done: true }
    const wrapper = mount(TodoItemCard, { props: { item } })
    const checkbox = wrapper.find('input[type="checkbox"]')
    expect(checkbox.element.checked).toBe(true)
  })

  it('applies line-through class when done', () => {
    const item = { ...defaultItem, done: true }
    const wrapper = mount(TodoItemCard, { props: { item } })
    const title = wrapper.find('.line-through')
    expect(title.exists()).toBe(true)
  })

  it('emits toggleDone with id and item', async () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })

    await wrapper.find('input[type="checkbox"]').trigger('change')

    expect(wrapper.emitted('toggleDone')).toHaveLength(1)
    expect(wrapper.emitted('toggleDone')![0]).toEqual(['item-1', defaultItem])
  })

  it('emits edit with item', async () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })

    await wrapper.findAll('button')[0].trigger('click')

    expect(wrapper.emitted('edit')).toHaveLength(1)
    expect(wrapper.emitted('edit')![0]).toEqual([defaultItem])
  })

  it('emits delete with item id', async () => {
    const wrapper = mount(TodoItemCard, { props: { item: defaultItem } })

    await wrapper.findAll('button')[1].trigger('click')

    expect(wrapper.emitted('delete')).toHaveLength(1)
    expect(wrapper.emitted('delete')![0]).toEqual(['item-1'])
  })
})
