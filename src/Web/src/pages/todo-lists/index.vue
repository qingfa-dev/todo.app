<template>
  <div>
    <div class="flex justify-between items-center mb-8">
      <h1 class="text-2xl font-bold text-gray-900">My Lists</h1>
      <button
        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 font-medium text-sm"
        @click="showCreateForm = true"
      >
        + New List
      </button>
    </div>

    <TodoListsTodoListForm
      v-if="showCreateForm"
      ref="todoListFormRef"
      @submit="handleCreate"
      @cancel="showCreateForm = false"
    />

    <TodoListsTodoListForm
      v-if="editingList"
      ref="todoListEditFormRef"
      :list="editingList"
      @submit="handleUpdate"
      @cancel="editingList = null"
    />

    <div v-if="store.loading && store.items.length === 0" class="text-center py-12 text-gray-500">
      Loading...
    </div>

    <div v-else-if="store.items.length === 0" class="text-center py-12">
      <p class="text-gray-500 mb-4">No todo lists yet</p>
      <button
        class="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 text-sm"
        @click="showCreateForm = true"
      >
        Create your first list
      </button>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      <TodoListsTodoListCard
        v-for="list in store.items"
        :key="list.id"
        :list="list"
        @edit="handleEdit"
        @delete="handleDelete"
      />
    </div>

    <CommonPagination
      v-if="store.totalPages > 1"
      :page="store.page"
      :total-pages="store.totalPages"
      :has-previous="store.hasPreviousPage"
      :has-next="store.hasNextPage"
      @page-change="handlePageChange"
    />

    <ConfirmModal
      v-if="deleteTargetId"
      title="Delete list?"
      message="This will permanently delete this list and all its items."
      confirm-text="Delete"
      @confirm="confirmDelete"
      @cancel="deleteTargetId = null"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useTodoListsStore } from '@/stores/todoLists'
import TodoListsTodoListForm from '@/components/todo-lists/TodoListForm.vue'
import TodoListsTodoListCard from '@/components/todo-lists/TodoListCard.vue'
import CommonPagination from '@/components/common/Pagination.vue'
import ConfirmModal from '@/components/common/ConfirmModal.vue'
import type { TodoList } from '@/types/todoList'

const store = useTodoListsStore()
const showCreateForm = ref(false)
const todoListFormRef = ref<InstanceType<typeof TodoListsTodoListForm> | null>(null)
const todoListEditFormRef = ref<InstanceType<typeof TodoListsTodoListForm> | null>(null)
const editingList = ref<TodoList | null>(null)
const deleteTargetId = ref<string | null>(null)

onMounted(() => {
  store.fetchLists()
})

onUnmounted(() => {
  store.reset()
})

async function handleCreate(data: { title: string; colour: string }) {
  try {
    await store.createList(data)
    showCreateForm.value = false
  } catch (error: unknown) {
    todoListFormRef.value?.setError(error)
  }
}

function handleEdit(list: TodoList) {
  showCreateForm.value = false
  editingList.value = list
}

async function handleUpdate(data: { title: string; colour: string }) {
  if (!editingList.value) return
  try {
    await store.updateList(editingList.value.id, data)
    editingList.value = null
  } catch (error: unknown) {
    todoListEditFormRef.value?.setError(error)
  }
}

function handleDelete(id: string) {
  deleteTargetId.value = id
}

async function confirmDelete() {
  if (deleteTargetId.value) {
    await store.deleteList(deleteTargetId.value)
    deleteTargetId.value = null
  }
}

function handlePageChange(page: number) {
  store.fetchLists({ page })
}
</script>
