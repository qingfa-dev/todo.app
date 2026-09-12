import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createRouter, createMemoryHistory } from 'vue-router'
import TodoListCard from '@/components/todo-lists/TodoListCard.vue'

describe('TodoListCard', () => {
  const defaultList = {
    id: 'list-1',
    title: 'My Todo List',
    colour: '#E05C4D',
  }

  async function mountWithRouter(list = defaultList) {
    const router = createRouter({
      history: createMemoryHistory(),
      routes: [
        { path: '/', component: {} },
        { path: '/todo-lists/:id', component: {} },
      ],
    })
    router.push('/')
    await router.isReady()

    return mount(TodoListCard, {
      props: { list },
      global: { plugins: [router] },
    })
  }

  it('renders list title', async () => {
    const wrapper = await mountWithRouter()
    expect(wrapper.text()).toContain('My Todo List')
  })

  it('renders colour indicator with hex value', async () => {
    const wrapper = await mountWithRouter()
    const indicator = wrapper.find('.rounded-full')
    expect(indicator.attributes('style')).toContain('#E05C4D')
  })

  it('emits edit event with list', async () => {
    const wrapper = await mountWithRouter()

    await wrapper.findAll('button')[0].trigger('click')

    expect(wrapper.emitted('edit')).toHaveLength(1)
    expect(wrapper.emitted('edit')![0]).toEqual([defaultList])
  })

  it('emits delete event with list id', async () => {
    const wrapper = await mountWithRouter()

    await wrapper.findAll('button')[1].trigger('click')

    expect(wrapper.emitted('delete')).toHaveLength(1)
    expect(wrapper.emitted('delete')![0]).toEqual(['list-1'])
  })

  it('does not emit delete when clicking card body', async () => {
    const wrapper = await mountWithRouter()

    await wrapper.find('.cursor-pointer').trigger('click')

    expect(wrapper.emitted('delete')).toBeUndefined()
  })

  it('has cursor-pointer class for navigation', async () => {
    const wrapper = await mountWithRouter()
    const card = wrapper.find('.cursor-pointer')
    expect(card.exists()).toBe(true)
    expect(card.attributes('class')).toContain('hover:shadow-md')
  })
})
