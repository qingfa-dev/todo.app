import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import ConfirmModal from '@/components/common/ConfirmModal.vue'

describe('ConfirmModal', () => {
  const globalStubs = {
    teleport: true,
  }

  it('renders title and message', () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Delete item?', message: 'This cannot be undone.' },
      global: { stubs: globalStubs },
    })

    expect(wrapper.text()).toContain('Delete item?')
    expect(wrapper.text()).toContain('This cannot be undone.')
  })

  it('renders default confirm text', () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Delete?', message: 'Sure?' },
      global: { stubs: globalStubs },
    })

    expect(wrapper.text()).toContain('Delete')
  })

  it('renders custom confirm text', () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Remove?', message: 'Sure?', confirmText: 'Remove' },
      global: { stubs: globalStubs },
    })

    expect(wrapper.text()).toContain('Remove')
  })

  it('renders cancel button', () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Delete?', message: 'Sure?' },
      global: { stubs: globalStubs },
    })

    expect(wrapper.text()).toContain('Cancel')
  })

  it('emits confirm when confirm button clicked', async () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Delete?', message: 'Sure?' },
      global: { stubs: globalStubs },
    })

    const confirmButton = wrapper.findAll('button').find((b) => b.text() === 'Delete')
    await confirmButton!.trigger('click')

    expect(wrapper.emitted('confirm')).toHaveLength(1)
  })

  it('emits cancel when cancel button clicked', async () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Delete?', message: 'Sure?' },
      global: { stubs: globalStubs },
    })

    const cancelButton = wrapper.findAll('button').find((b) => b.text() === 'Cancel')
    await cancelButton!.trigger('click')

    expect(wrapper.emitted('cancel')).toHaveLength(1)
  })

  it('emits cancel when backdrop clicked', async () => {
    const wrapper = mount(ConfirmModal, {
      props: { title: 'Delete?', message: 'Sure?' },
      global: { stubs: globalStubs },
    })

    const backdrop = wrapper.find('.fixed.inset-0.bg-black\\/50')
    await backdrop.trigger('click')

    expect(wrapper.emitted('cancel')).toHaveLength(1)
  })
})
