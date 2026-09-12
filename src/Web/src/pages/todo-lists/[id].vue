<template>
  <div>
    <div class="mb-6">
      <RouterLink to="/todo-lists" class="text-indigo-600 hover:text-indigo-800 text-sm font-medium">
        ← Back to Lists
      </RouterLink>
    </div>

    <div v-if="loading" class="text-center py-12 text-gray-500">Loading...</div>

    <template v-else>
      <div class="flex justify-between items-center mb-8">
        <div class="flex items-center gap-3">
          <div
            class="w-4 h-4 rounded-full"
            :style="{ backgroundColor: list?.colour }"
          />
          <h1 class="text-2xl font-bold text-gray-900">{{ list?.title }}</h1>
        </div>
        <button
          class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 font-medium text-sm"
          @click="showCreateForm = true"
        >
          + Add Item
        </button>
      </div>

      <TodoItemsTodoItemForm
        v-if="showCreateForm"
        ref="todoItemFormRef"
        :list-id="listId"
        @submit="handleCreateItem"
        @cancel="showCreateForm = false"
      />

      <TodoItemsTodoItemForm
        v-if="editingItem"
        ref="todoItemEditFormRef"
        :list-id="listId"
        :item="editingItem"
        @submit="handleUpdateItem"
        @cancel="editingItem = null"
      />

      <div v-if="itemsStore.loading && itemsStore.items.length === 0" class="text-center py-12 text-gray-500">
        Loading items...
      </div>

      <div v-else-if="itemsStore.items.length === 0" class="text-center py-12">
        <p class="text-gray-500">No items in this list</p>
      </div>

      <div v-else class="space-y-3">
        <TodoItemsTodoItemCard
          v-for="item in itemsStore.items"
          :key="item.id"
          :item="item"
          @toggle-done="handleToggleDone"
          @edit="handleEditItem"
          @delete="handleDeleteItem"
        />
      </div>

      <CommonPagination
        v-if="itemsStore.totalPages > 1"
        :page="itemsStore.page"
        :total-pages="itemsStore.totalPages"
        :has-previous="itemsStore.hasPreviousPage"
        :has-next="itemsStore.hasNextPage"
        @page-change="handlePageChange"
      />
    </template>

    <ConfirmModal
      v-if="deleteTargetId"
      title="Delete item?"
      message="This action cannot be undone."
      confirm-text="Delete"
      @confirm="confirmDelete"
      @cancel="deleteTargetId = null"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { useTodoLists } from '@/composables/useTodoLists'
import { useTodoItemsStore } from '@/stores/todoItems'
import type { TodoList } from '@/types/todoList'
import type { TodoItem } from '@/types/todoItem'
import TodoItemsTodoItemForm from '@/components/todo-items/TodoItemForm.vue'
import TodoItemsTodoItemCard from '@/components/todo-items/TodoItemCard.vue'
import CommonPagination from '@/components/common/Pagination.vue'
import ConfirmModal from '@/components/common/ConfirmModal.vue'

const route = useRoute()
const listId = route.params.id as string
const itemsStore = useTodoItemsStore()
const api = useTodoLists()

const list = ref<TodoList | null>(null)
const loading = ref(true)
const showCreateForm = ref(false)
const todoItemFormRef = ref<InstanceType<typeof TodoItemsTodoItemForm> | null>(null)
const todoItemEditFormRef = ref<InstanceType<typeof TodoItemsTodoItemForm> | null>(null)
const editingItem = ref<TodoItem | null>(null)
const deleteTargetId = ref<string | null>(null)

onMounted(async () => {
  try {
    list.value = await api.fetchList(listId)
    await itemsStore.fetchItems({ listId })
  } finally {
    loading.value = false
  }
})

onUnmounted(() => {
  itemsStore.reset()
})

async function handleCreateItem(data: { title: string; note?: string; priority?: number }) {
  try {
    await itemsStore.createItem({ ...data, listId })
    showCreateForm.value = false
  } catch (error: unknown) {
    todoItemFormRef.value?.setError(error)
  }
}

function handleEditItem(item: TodoItem) {
  showCreateForm.value = false
  editingItem.value = item
}

async function handleUpdateItem(data: { title: string; note?: string; priority?: number }) {
  if (!editingItem.value) return
  try {
    await itemsStore.updateItem(editingItem.value.id, { ...data, listId })
    editingItem.value = null
  } catch (error: unknown) {
    todoItemEditFormRef.value?.setError(error)
  }
}

async function handleToggleDone(id: string, item: any) {
  await itemsStore.toggleDone(id, item)
}

function handleDeleteItem(id: string) {
  deleteTargetId.value = id
}

async function confirmDelete() {
  if (deleteTargetId.value) {
    await itemsStore.deleteItem(deleteTargetId.value)
    deleteTargetId.value = null
  }
}

function handlePageChange(page: number) {
  itemsStore.fetchItems({ listId, page })
}
</script>
