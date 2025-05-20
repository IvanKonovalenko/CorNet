<template>
  <div class="workspace">
    <h2 class="section-title">Профиль</h2>
    <div class="card">
      <p><strong>Имя:</strong> {{ user.name }}</p>
      <p><strong>Фамилия:</strong> {{ user.surname }}</p>
      <p><strong>Email:</strong> {{ user.email }}</p>
    </div>

    <h2 class="section-title">Мои организации</h2>
    <div class="card card-list">
      <div
        v-for="org in organizations"
        :key="org.code"
        @click="goToOrganization(org.code)"
        class="card-item"
      >
        <div class="card-title">{{ org.name }}</div>
        <div class="card-subtext">Код: {{ org.code }}</div>
      </div>
    </div>

    <div class="card">
      <h3 class="subsection-title">Отправить запрос на вступление</h3>
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

      <h3 class="subsection-title">Создать организацию</h3>
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

    <button class="logout-button" @click="logout">Выйти</button>
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
    await sendRequest({ headers: { Authorization: `Bearer ${token}` } }, joinCode.value)
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
    await create(
      { name: newOrgName.value, code: newOrgCode.value },
      { headers: { Authorization: `Bearer ${token}` } }
    )
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
  max-width: 900px;
  margin: 60px auto;
  padding: 32px;
  font-family: 'Segoe UI', sans-serif;
  color: #2c3e50;
}

.section-title {
  font-size: 28px;
  font-weight: 700;
  color: #3f51b5;
  margin-bottom: 20px;
}

.subsection-title {
  font-size: 20px;
  font-weight: 600;
  color: #3f51b5;
  margin: 24px 0 12px;
}

.card {
  background-color: #ffffff;
  border-radius: 16px;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.06);
  padding: 24px;
  margin-bottom: 28px;
}

.card-list {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.card-item {
  background-color: #f4f6ff;
  border: 1px solid #d4dbff;
  border-radius: 14px;
  padding: 18px;
  transition: all 0.2s ease;
  cursor: pointer;
}
.card-item:hover {
  background-color: #e3e7ff;
  box-shadow: 0 4px 12px rgba(63, 81, 181, 0.15);
}

.card-title {
  font-size: 17px;
  font-weight: 600;
  margin-bottom: 6px;
}

.card-subtext {
  font-size: 14px;
  color: #666e91;
}

input {
  width: 100%;
  padding: 12px;
  margin-top: 10px;
  margin-bottom: 12px;
  border-radius: 10px;
  border: 1px solid #ccc;
  font-size: 15px;
  transition: border-color 0.3s ease;
}
input:focus {
  outline: none;
  border-color: #3f51b5;
}

button {
  width: 100%;
  padding: 12px;
  background-color: #3f51b5;
  color: #fff;
  border: none;
  font-weight: 600;
  border-radius: 10px;
  font-size: 15px;
  cursor: pointer;
  transition: background-color 0.3s ease;
  margin-bottom: 8px;
}

button:disabled {
  background-color: #b0b0b0;
  cursor: not-allowed;
}

button:hover:enabled {
  background-color: #2c3db4;
}

.logout-button {
  background-color: #e53935;
  margin-top: 40px;
}

.logout-button:hover {
  background-color: #c62828;
}

.error {
  color: #e53935;
  margin-top: 12px;
  font-weight: 500;
}

.loading {
  color: #3f51b5;
  margin-top: 12px;
  font-style: italic;
}
</style>
