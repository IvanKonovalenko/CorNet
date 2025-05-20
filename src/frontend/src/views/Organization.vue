<template>
  <div class="organization-container">
    <h1>Организация: {{ orgCode }}</h1>

    <section class="section">
      <h2 @click="showUsersBlock = !showUsersBlock" style="cursor: pointer">
        Пользователи <span>{{ showUsersBlock ? '🔽' : '▶️' }}</span>
      </h2>
      <div v-if="showUsersBlock">
        <div v-if="userError" class="error-message">{{ userError }}</div>
        <div v-for="user in users" :key="user.email" class="card" style="cursor: pointer">
          <div class="card-title" @click="goToUser(user.email)">
            {{ user.name }} {{ user.surname }} ({{ user.email }}) — <strong>{{ user.role }}</strong>
          </div>
          <div class="card-actions">
            <button @click.stop="confirmRemoveUser(user.email)" class="danger">Удалить</button>
            <select v-model="user.role" @click.stop @change="confirmChangeUserRole(user.email, user.role)">
              <option value="Member">Member</option>
              <option value="Admin">Admin</option>
              <option value="Owner" disabled>Owner</option>
            </select>
          </div>
        </div>
      </div>
    </section>

    <section class="section" v-if="isVisible">
      <h2 @click="showRequestsBlock = !showRequestsBlock" style="cursor: pointer">
        Запросы на вступление <span>{{ showRequestsBlock ? '🔽' : '▶️' }}</span>
      </h2>
      <div v-if="showRequestsBlock">
        <div v-if="joinRequests.length">
          <div v-for="request in joinRequests" :key="request.id" class="card">
            <div class="card-title">{{ request.name }} {{ request.surname }} ({{ request.email }})</div>
            <div class="card-actions">
              <button @click="confirmAcceptRequest(request.organizationRequestId)">Принять</button>
              <button @click="confirmRejectRequest(request.organizationRequestId)" class="danger">Отклонить</button>
            </div>
          </div>
        </div>
        <div v-else>Нет новых запросов.</div>
      </div>
    </section>

    <section class="section">
      <h2>Создать пост</h2>
      <textarea v-model="newPostText" placeholder="Напишите новый пост"></textarea>
      <div v-if="errorPost" class="error-message">{{ errorPost }}</div>
      <div v-if="postSuccess" class="success-message">{{ postSuccess }}</div>
      <div class="card-actions">
        <button @click="createNewPost">Отправить</button>
      </div>
    </section>

    <section class="section">
      <h2>Посты</h2>
      <div v-if="postDeleteSuccess" class="success-message">{{ postDeleteSuccess }}</div>
      <div v-if="postError" class="error-message">{{ postError }}</div>

      <div v-for="post in posts" :key="post.postId" class="card">
        <div class="card-title">
          <small>Создано: {{ formatDate(post.dateTime) }} автор: {{ post.email }}</small>
        </div>
        <p>{{ post.text }}</p>

        <div class="post-actions">
          <div class="likes">
          ❤️ {{ post.likes }} лайков
          <button :class="['like-button', post.isLiked ? 'liked' : 'not-liked']" @click="toggleLike(post.postId)">
            {{ post.isLiked ? '💔 Убрать лайк' : '👍 Лайкнуть' }}
          </button>
          </div>
          <div>
            <button class="danger" @click="confirmDeletePost(post.postId)">🗑 Удалить пост</button>
          </div>
        </div>


        <div class="comments">
          <h4>Комментарии</h4>

          <div v-for="comment in post.comments" :key="comment.commentId" class="comment">
            <div class="comment-text">
              <p>{{ comment.text }}</p>
              <small>{{ formatDate(comment.dateTime) }} — {{ comment.email }}</small>
            </div>
            <button @click="removeComment(post.postId, comment.commentId)" class="danger">🗑</button>
          </div>

          <div v-if="commentSuccess[post.postId]" class="success-message">{{ commentSuccess[post.postId] }}</div>
          <div v-if="commentError[post.postId]" class="error-message">{{ commentError[post.postId] }}</div>

          <textarea v-model="newComments[post.postId]" placeholder="Оставить комментарий"></textarea>
          <div class="card-actions">
            <button @click="addComment(post.postId)">Отправить</button>
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
  showComments, createComment, deleteComment,
  deletePost
} from '../services/postcontrol'

const props = defineProps({
  orgCode: { type: String, required: true }
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

const users = ref([])
const userError = ref('')
const joinRequests = ref([])
const posts = ref([])

const isVisible = ref(false)
const showUsersBlock = ref(true)
const showRequestsBlock = ref(true)

const newPostText = ref('')
const errorPost = ref('')
const postSuccess = ref('')
const postError = ref('')
const postDeleteSuccess = ref('')

const newComments = ref({})
const commentSuccess = ref({})
const commentError = ref({})

onMounted(async () => {
  await fetchUsers()
  await fetchRequests()
  await fetchPosts()
})

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

async function confirmRemoveUser(email) {
  if (confirm(`Вы уверены, что хотите удалить пользователя ${email}?`)) {
    await removeUser(email)
  }
}

async function removeUser(emailToDelete) {
  try {
    await deleteUser(props.orgCode, emailToDelete, { headers: { Authorization: `Bearer ${token}` } })
    await fetchUsers()
  } catch (err) {
    userError.value = err.response?.data?.error || 'Ошибка при удалении пользователя.'
  }
}

async function confirmChangeUserRole(email, role) {
  if (confirm(`Вы уверены, что хотите изменить роль пользователя ${email} на ${role}?`)) {
    await changeUserRole(email, role)
  }
}

async function changeUserRole(emailToChange, newRole) {
  try {
    await changeRoleUser(props.orgCode, emailToChange, newRole, { headers: { Authorization: `Bearer ${token}` } })
    await fetchUsers()
  } catch (err) {
    userError.value = err.response?.data?.error || 'Ошибка при изменении роли.'
  }
}

async function fetchRequests() {
  try {
    const res = await showRequests(props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
    joinRequests.value = res.data
    isVisible.value = true
  } catch (error) {
    const msg = error?.response?.data?.error
    if (msg === 'Просмотреть запросы может только владелец или администратор') {
      isVisible.value = false
      joinRequests.value = []
    } else {
      console.error('Ошибка при загрузке запросов:', msg || error.message)
    }
  }
}

async function confirmAcceptRequest(id) {
  if (confirm('Вы уверены, что хотите принять этот запрос на вступление?')) {
    await acceptJoinRequest(id)
  }
}

async function confirmRejectRequest(id) {
  if (confirm('Вы уверены, что хотите отклонить этот запрос на вступление?')) {
    await rejectJoinRequest(id)
  }
}

async function acceptJoinRequest(id) {
  try {
    await acceptRequest(id, props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
    await fetchRequests()
    await fetchUsers()
  } catch (error) {
    console.error('Ошибка при принятии запроса:', error)
  }
}

async function rejectJoinRequest(id) {
  try {
    await refuseRequest(id, props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
    await fetchRequests()
  } catch (error) {
    console.error('Ошибка при отклонении запроса:', error)
  }
}

async function createNewPost() {
  errorPost.value = ''
  postSuccess.value = ''
  if (newPostText.value.trim()) {
    try {
      await createPost(newPostText.value, props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
      newPostText.value = ''
      postSuccess.value = 'Пост успешно создан.'
      await fetchPosts()
    } catch (error) {
      errorPost.value = error.response?.data?.error || 'Ошибка при отправке поста.'
    }
  } else {
    errorPost.value = 'Пост не может быть пустым.'
  }
}

async function fetchPosts() {
  const res = await showPosts(props.orgCode, { headers: { Authorization: `Bearer ${token}` } })
  posts.value = res.data
  for (const post of posts.value) {
    const resComments = await showComments(props.orgCode, post.postId, { headers: { Authorization: `Bearer ${token}` } })
    post.comments = resComments.data
    newComments.value[post.postId] = ''
  }
}

async function addComment(postId) {
  const comment = newComments.value[postId]?.trim()
  commentError.value[postId] = ''
  commentSuccess.value[postId] = ''

  if (comment) {
    try {
      await createComment(props.orgCode, postId, comment, { headers: { Authorization: `Bearer ${token}` } })
      newComments.value[postId] = ''
      commentSuccess.value[postId] = 'Комментарий добавлен.'
      await fetchPosts()
    } catch (err) {
      commentError.value[postId] = 'Ошибка при добавлении комментария.'
    }
  } else {
    commentError.value[postId] = 'Комментарий не может быть пустым.'
  }
}

async function removeComment(postId, commentId) {
  commentError.value[postId] = ''
  commentSuccess.value[postId] = ''

  try {
    await deleteComment(props.orgCode, postId, commentId, { headers: { Authorization: `Bearer ${token}` } })
    commentSuccess.value[postId] = 'Комментарий удален.'
    await fetchPosts()
  } catch (err) {
    commentError.value[postId] = 'Ошибка при удалении комментария.'
  }
}

async function toggleLike(postId) {
  const post = posts.value.find(p => p.postId === postId)
  if (!post) return
  try {
    if (post.isLiked) {
      await disLike(props.orgCode, postId, { headers: { Authorization: `Bearer ${token}` } })
    } else {
      await like(props.orgCode, postId, { headers: { Authorization: `Bearer ${token}` } })
    }
    await fetchPosts()
  } catch (error) {
    console.error('Ошибка при обработке лайка:', error)
  }
}

async function confirmDeletePost(postId) {
  postError.value = ''
  postDeleteSuccess.value = ''
  if (confirm('Вы уверены, что хотите удалить этот пост?')) {
    try {
      await deletePost(props.orgCode, postId, { headers: { Authorization: `Bearer ${token}` } })
      postDeleteSuccess.value = 'Пост успешно удалён.'
      await fetchPosts()
    } catch (err) {
      postError.value = err.response?.data?.error || 'Ошибка при удалении поста.'
    }
  }
}

function formatDate(dateStr) {
  const date = new Date(dateStr)
  return date.toLocaleString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.organization-container {
  padding: 32px;
  max-width: 900px;
  margin: auto;
  font-family: 'Segoe UI', sans-serif;
  background-color: #f9fafb;
  color: #2c3e50;
}

.section {
  margin-bottom: 32px;
  padding: 24px;
  background-color: #ffffff;
  border-radius: 16px;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.06);
}

.section h2 {
  font-size: 28px;
  margin-bottom: 16px;
  font-weight: 700;
  color: #3f51b5;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.card {
  background-color: #f4f6ff;
  border: 1px solid #d4dbff;
  padding: 18px;
  border-radius: 14px;
  margin-top: 12px;
  transition: all 0.2s ease;
}

.card:hover {
  background-color: #e3e7ff;
  box-shadow: 0 4px 12px rgba(63, 81, 181, 0.15);
}

.card-title {
  font-weight: 600;
  font-size: 17px;
  color: #2c3e50;
  margin-bottom: 8px;
}

.card-actions {
  display: flex;
  gap: 12px;
  margin-top: 12px;
  flex-wrap: wrap;
}

textarea {
  width: 100%;
  padding: 12px;
  border-radius: 10px;
  border: 1px solid #ccc;
  font-size: 15px;
  resize: vertical;
  box-sizing: border-box;
  margin-top: 8px;
  font-family: inherit;
  background-color: #ffffff;
  color: #2c3e50;
}

.success-message {
  color: #4caf50;
  margin-top: 8px;
  font-weight: 500;
}

.error-message {
  color: #e53935;
  margin-top: 8px;
  font-weight: 500;
}

.comment {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 12px;
  padding: 8px 0;
  border-top: 1px solid #d4dbff;
}

.comment-text {
  flex: 1;
  font-size: 15px;
  color: #666e91;
}

.comment button {
  flex-shrink: 0;
  background-color: #e53935;
  color: white;
  padding: 6px 14px;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  font-size: 14px;
}

.post-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 12px;
  flex-wrap: wrap;
  gap: 10px;
}

.likes {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 15px;
  color: #666e91;
}

button {
  padding: 12px;
  background-color: #3f51b5;
  color: #fff;
  border: none;
  font-weight: 600;
  border-radius: 10px;
  font-size: 15px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #2c3db4;
}

button.danger {
  background-color: #e53935;
}

button.danger:hover {
  background-color: #c62828;
}

.like-button {
  padding: 8px 14px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  color: white;
  font-size: 15px;
  transition: background 0.2s ease;
}

.like-button.liked {
  background-color: #e53935;
}

.like-button.not-liked {
  background-color: #b0b0b0;
}
</style>
