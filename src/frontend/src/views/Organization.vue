<template>
  <div class="organization-container">
    <h1>Организация: {{ orgCode }}</h1>

    <!-- Пользователи -->
    <section class="section">
    <h2>Пользователи</h2>
    <div v-if="userError" class="error-message">{{ userError }}</div>
    <div v-for="user in users" :key="user.email" class="card" style="cursor: pointer">
    <div class="card-title" @click="goToUser(user.email)">
      {{ user.name }} ({{ user.email }}) — <strong>{{ user.role }}</strong>
    </div>
    <div class="card-actions">
      <button @click.stop="removeUser(user.email)" class="danger">Удалить</button>
      <select
        v-model="user.role"
        @click.stop
        @change="changeUserRole(user.email, user.role)">
        <option value="Member">Member</option>
        <option value="Admin">Admin</option>
        <option value="Owner" disabled>Owner</option>
      </select>
    </div>
  </div>
</section>


    <!-- Запросы на вступление -->
    <section class="section">
      <h2>Запросы на вступление</h2>
      <div v-if="joinRequests.length">
        <div v-for="request in joinRequests" :key="request.id" class="card">
          <div class="card-title">{{ request.name }}</div>
          <div class="card-actions">
            <button @click="acceptJoinRequest(request.id)">Принять</button>
            <button @click="rejectJoinRequest(request.id)" class="danger">Отклонить</button>
          </div>
        </div>
      </div>
      <div v-else>Нет новых запросов.</div>
    </section>

    <!-- Создание поста -->
    <section class="section">
      <h2>Создать пост</h2>
      <textarea v-model="newPostText" placeholder="Напишите новый пост"></textarea>
      <div class="card-actions">
        <button @click="createNewPost">Отправить</button>
      </div>
    </section>

    <!-- Посты -->
    <section class="section">
      <h2>Посты</h2>
      <div v-for="post in posts" :key="post.id" class="card">
        <div class="card-title">{{ post.title }}</div>
        <p>{{ post.content }}</p>
        <div class="likes">
          ❤️ {{ post.likes }} лайков
          <button @click="toggleLike(post.id)">👍 Лайк/Анлайк</button>
        </div>

        <div class="comments">
          <h4>Комментарии</h4>
          <div v-for="comment in post.comments" :key="comment.id" class="comment">
            {{ comment.text }}
          </div>
          <textarea v-model="newComments[post.id]" placeholder="Оставить комментарий"></textarea>
          <div class="card-actions">
            <button @click="addComment(post.id)">Отправить</button>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { defineProps } from 'vue'
import { useRouter } from 'vue-router'
import {
  showRequests, acceptRequest, refuseRequest,
  showUsers, deleteUser, changeRoleUser
} from '../services/organizationcontrol'
import {
  createPost, showPosts, like, disLike,
  showComments, createComment
} from '../services/postcontrol'

const props = defineProps({
  orgCode: {
    type: String,
    required: true
  }
})

const router = useRouter()
const token = localStorage.getItem('token')
const email = parseJwt(token)?.idemail

function parseJwt(token) {
  if (!token) return null
  try {
    return JSON.parse(atob(token.split('.')[1]))
  } catch (e) {
    return null
  }
}

// State
const users = ref([])
const userError = ref('')
const joinRequests = ref([])
const posts = ref([])
const newPostText = ref('')
const newComments = ref({})

// Lifecycle
onMounted(async () => {
  await fetchUsers()
  await fetchRequests()
  await fetchPosts()
})

// Пользователи
async function fetchUsers() {
  try {
    userError.value = ''
    const res = await showUsers(props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
    users.value = res.data
  } catch (err) {
    userError.value = err.response?.data?.error || 'Ошибка при загрузке пользователей.'
  }
}

function goToUser(email) {
  router.push(`/user/${email}`)
}

async function removeUser(emailToDelete) {
  try {
    userError.value = ''
    await deleteUser(props.orgCode, emailToDelete, { headers: { Authorization: `Bearer ${token}` } })
    await fetchUsers()
  } catch (err) {
    userError.value = err.response?.data?.error || 'Ошибка при удалении пользователя.'
  }
}

async function changeUserRole(emailToChange, newRole) {
  try {
    userError.value = ''
    await changeRoleUser(props.orgCode, emailToChange, newRole, { headers: { Authorization: `Bearer ${token}` } })
    await fetchUsers()
  } catch (err) {
    userError.value = err.response?.data?.error || 'Ошибка при изменении роли.'
  }
}

// Запросы на вступление
async function fetchRequests() {
  const res = await showRequests(props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
  joinRequests.value = res.data
}

async function acceptJoinRequest(id) {
  await acceptRequest(id, props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
  await fetchRequests()
  await fetchUsers()
}

async function rejectJoinRequest(id) {
  await refuseRequest(id, props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
  await fetchRequests()
}

// Посты
async function fetchPosts() {
  const res = await showPosts(props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
  posts.value = res.data
  for (const post of posts.value) {
    const resComments = await showComments(props.orgCode, post.id, { headers: { Authorization: `Bearer ${token}` } })
    post.comments = resComments.data
    newComments.value[post.id] = ''
  }
}

async function createNewPost() {
  if (newPostText.value.trim()) {
    await createPost(newPostText.value, props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
    newPostText.value = ''
    await fetchPosts()
  }
}

async function addComment(postId) {
  const comment = newComments.value[postId]?.trim()
  if (comment) {
    await createComment(props.orgCode, postId, comment, { headers: { Authorization: `Bearer ${token}` } })
    newComments.value[postId] = ''
    await fetchPosts()
  }
}

async function toggleLike(postId) {
  const post = posts.value.find(p => p.id === postId)
  if (!post) return
  if (post.likedByUser) {
    await disLike(props.orgCode, postId, { headers: { Authorization: `Bearer ${token}` } })
    post.likedByUser = false
  } else {
    await like(props.orgCode, postId, { headers: { Authorization: `Bearer ${token}` } })
    post.likedByUser = true
  }
  await fetchPosts()
}
</script>

<style scoped>
.organization-container {
  padding: 16px;
  max-width: 1200px;
  margin: 0 auto;
}

.section {
  margin-bottom: 24px;
}

.section h2 {
  margin-bottom: 12px;
  font-size: 20px;
  color: #333;
}

.card {
  background-color: #f9f9f9;
  padding: 12px 16px;
  border: 1px solid #ddd;
  border-radius: 8px;
  margin-bottom: 12px;
}

.card-title {
  font-weight: bold;
  font-size: 16px;
  margin-bottom: 6px;
}

.card-actions {
  display: flex;
  gap: 8px;
  margin-top: 8px;
}

button {
  padding: 6px 10px;
  font-size: 14px;
  border-radius: 4px;
  border: none;
  cursor: pointer;
  background-color: #4285f4;
  color: white;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #3367d6;
}

button.danger {
  background-color: #e53935;
}

button.danger:hover {
  background-color: #c62828;
}

textarea {
  width: 100%;
  padding: 8px;
  border-radius: 4px;
  border: 1px solid #ccc;
  resize: vertical;
  min-height: 60px;
  font-size: 14px;
}

.comments {
  margin-top: 12px;
}

.comment {
  padding: 8px;
  border: 1px solid #eee;
  border-radius: 6px;
  margin-top: 8px;
  background-color: #fff;
}

.likes {
  margin-top: 8px;
  font-size: 14px;
  color: #666;
}

.error-message {
  color: #d32f2f;
  background: #ffebee;
  padding: 8px;
  margin-bottom: 12px;
  border: 1px solid #ef9a9a;
  border-radius: 6px;
}

@media (max-width: 768px) {
  .card-actions {
    flex-direction: column;
    gap: 4px;
  }
}
</style>
