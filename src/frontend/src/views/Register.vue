<template>
  <div class="auth-container">
    <div class="auth-card">
      <h2>Регистрация</h2>
      <form @submit.prevent="handleRegister">
        <input v-model="name" type="text" placeholder="Имя" required />
        <input v-model="surname" type="text" placeholder="Фамилия" required />
        <input v-model="email" type="email" placeholder="Email" required />
        <p v-if="emailError" class="error">{{ emailError }}</p>

        <input v-model="password" type="password" placeholder="Пароль" required />
        <p v-if="passwordError" class="error">{{ passwordError }}</p>

        <button type="submit" :disabled="loading">Зарегистрироваться</button>
        <p v-if="loading" class="loading">Загрузка...</p>
        <p v-if="error" class="error">{{ error }}</p>
      </form>
      <p class="switch">
        Уже есть аккаунт? <a @click="goToLogin">Войти</a>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { register } from '../services/auth'

const router = useRouter()
const name = ref('')
const surname = ref('')
const email = ref('')
const password = ref('')
const error = ref('')
const emailError = ref('')
const passwordError = ref('')
const loading = ref(false)

const validateEmail = (email) => {
  const emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/
  return emailPattern.test(email)
}

const validatePassword = (password) => {
  return password.length >= 8
}

const handleRegister = async () => {
  emailError.value = ''
  passwordError.value = ''

  if (!validateEmail(email.value)) {
    emailError.value = 'Неверный формат email'
    return
  }

  if (!validatePassword(password.value)) {
    passwordError.value = 'Пароль должен быть не менее 8 символов'
    return
  }

  loading.value = true

  try {
    await register({ name: name.value, surname: surname.value, email: email.value, password: password.value })
    router.push('/login')
  } catch (err) {
    error.value = err.response?.data?.error || 'Ошибка регистрации'
  } finally {
    loading.value = false
  }
}

const goToLogin = () => router.push('/login')
</script>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: #f4f6fc;
  font-family: 'Segoe UI', sans-serif;
}

.auth-card {
  background: #fff;
  padding: 32px;
  border-radius: 16px;
  box-shadow: 0 6px 24px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
}

.auth-card h2 {
  font-size: 26px;
  font-weight: 600;
  color: #3f51b5;
  margin-bottom: 24px;
  text-align: center;
}

input {
  width: 100%;
  padding: 12px;
  margin: 8px 0;
  border: 1px solid #ccc;
  border-radius: 10px;
  font-size: 16px;
  transition: border-color 0.2s;
}

input:focus {
  border-color: #3f51b5;
  outline: none;
}

button {
  width: 100%;
  padding: 12px;
  margin-top: 12px;
  background: #3f51b5;
  color: white;
  border: none;
  font-weight: 600;
  border-radius: 10px;
  font-size: 16px;
  cursor: pointer;
  transition: background 0.3s ease;
}

button:disabled {
  background: #bbb;
  cursor: not-allowed;
}

button:hover:enabled {
  background: #2c3db4;
}

.switch {
  margin-top: 20px;
  text-align: center;
  font-size: 14px;
}

.switch a {
  color: #3f51b5;
  cursor: pointer;
  text-decoration: underline;
}

.error {
  color: #e53935;
  font-size: 14px;
  margin: 6px 0 0 0;
}

.loading {
  color: #3f51b5;
  font-style: italic;
  font-size: 14px;
  margin-top: 8px;
}
</style>
