<template>
  <div class="workspace">
    <h2>Профиль</h2>
    <div class="profile">
      <p><strong>Имя:</strong> {{ user.name }}</p>
      <p><strong>Фамилия:</strong> {{ user.surname }}</p>
      <p><strong>Email:</strong> {{ user.email }}</p>
    </div>

    <h2>Мои организации</h2>
    <ul class="org-list">
      <li
        v-for="org in organizations"
        :key="org.code"
        @click="goToOrganization(org.code)"
        class="org-item"
      >
        {{ org.name }} (код: {{ org.code }})
      </li>
    </ul>

    <div class="actions">
      <h3>Отправить запрос на вступление</h3>
      <input
        v-model="joinCode"
        type="text"
        placeholder="Код организации"
      />
      <button
        @click="sendJoinRequest"
        :disabled="loading || !joinCode.trim()"
      >
        Отправить запрос
      </button>

      <h3>Создать организацию</h3>
      <input
        v-model="newOrgName"
        type="text"
        placeholder="Название организации"
      />
      <input
        v-model="newOrgCode"
        type="text"
        placeholder="Код организации"
      />
      <button
        @click="createOrganization"
        :disabled="loading || !newOrgName.trim() || !newOrgCode.trim()"
      >
        Создать
      </button>

      <p v-if="error" class="error">{{ error }}</p>
      <p v-if="loading" class="loading">Загрузка...</p>
    </div>

    <button class="logout" @click="logout">Выйти</button>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { showProfile, showOrganizations } from '../services/usercontrol'
import { create, sendRequest } from '../services/organizationcontrol'


const router = useRouter()
const user = ref({})
const organizations = ref([])
const joinCode = ref('')
const newOrgName = ref('')
const newOrgCode = ref('')
const error = ref('')
const loading = ref(false)

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

const fetchData = async () => {
  loading.value = true
  error.value = ''
  try {
    const profileRes = await showProfile({ params: { email } })
    user.value = profileRes.data

    const orgsRes = await showOrganizations({ params: { email } })
    organizations.value = orgsRes.data
  } catch (err) {
    error.value = err.response?.data?.error || 'Ошибка при загрузке данных'
  } finally {
    loading.value = false
  }
}

const sendJoinRequest = async () => {
  error.value = ''
  if (!joinCode.value.trim()) {
    error.value = 'Код организации не может быть пустым.'
    return
  }

  loading.value = true
  try {
    await sendRequest({ headers: { Authorization: `Bearer ${token}` } },joinCode.value)
    alert('Запрос отправлен')
    joinCode.value = ''
  } catch (err) {
    error.value = err.response?.data?.error || 'Ошибка при отправке запроса'
  } finally {
    loading.value = false
  }
}

const createOrganization = async () => {
  error.value = ''
  if (!newOrgName.value.trim() || !newOrgCode.value.trim()) {
    error.value = 'Название и код организации не могут быть пустыми.'
    return
  }

  loading.value = true
  try {
    await create({ name: newOrgName.value, code: newOrgCode.value },{ headers: { Authorization: `Bearer ${token}` } })
    alert('Организация создана')
    newOrgName.value = ''
    newOrgCode.value = ''
    fetchData()
  } catch (err) {
    error.value = err.response?.data?.error || 'Ошибка при создании организации'
  } finally {
    loading.value = false
  }
}

const goToOrganization = (code) => {
  router.push(`/organization/${code}`)
}

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}

onMounted(fetchData)
</script>

<style scoped>
.workspace {
  max-width: 700px;
  margin: 60px auto;
  padding: 30px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.05);
  font-family: 'Segoe UI', sans-serif;
}

.profile, .actions {
  margin-bottom: 30px;
}

h2, h3 {
  color: #3f51b5;
  margin-bottom: 10px;
}

.org-list {
  list-style: none;
  padding: 0;
}

.org-item {
  padding: 10px;
  margin-bottom: 8px;
  border: 1px solid #ddd;
  border-radius: 10px;
  cursor: pointer;
  transition: background 0.2s;
}

.org-item:hover {
  background-color: #f0f0f0;
}

input {
  width: 100%;
  padding: 10px;
  margin: 8px 0;
  border-radius: 8px;
  border: 1px solid #ccc;
}

button {
  width: 100%;
  padding: 10px;
  margin-top: 8px;
  background: #3f51b5;
  color: white;
  border: none;
  font-weight: bold;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
}

button:disabled {
  background: #aaa;
  cursor: not-allowed;
}

button:hover:enabled {
  background: #2c3db4;
}

.logout {
  background: #e53935;
}

.logout:hover {
  background: #c62828;
}

.error {
  color: red;
  margin-top: 10px;
}

.loading {
  color: #3f51b5;
  margin-top: 10px;
  font-style: italic;
}
</style>
