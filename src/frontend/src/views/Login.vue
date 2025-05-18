<template>
  <div class="auth-container">
    <div class="auth-card">
      <h2>Вход</h2>
      <form @submit.prevent="handleLogin">
        <input v-model="email" type="email" placeholder="Email" required />
        <p v-if="emailError" class="error">{{ emailError }}</p>

        <input v-model="password" type="password" placeholder="Пароль" required />
        <p v-if="passwordError" class="error">{{ passwordError }}</p>

        <button type="submit" :disabled="loading">Войти</button>
        <p v-if="loading" class="loading">Загрузка...</p>
        <p v-if="error" class="error">{{ error }}</p>
      </form>
      <p class="switch">
        Нет аккаунта? <a @click="goToRegister">Регистрация</a>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login } from '../services/auth'

const router = useRouter()
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

const handleLogin = async () => {
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
    const userData = {
      email: email.value,
      password: password.value,
    }

    const res = await login(userData)

    if (res.status === 200) {
      localStorage.setItem('token', res.data)
      router.push('/workspace')
    } else {
      error.value = 'Ошибка авторизации'
    }
  } catch (err) {
    error.value = err.response?.data?.error || 'Ошибка входа'
  } finally {
    loading.value = false  // Скрываем спиннер
  }
}

const goToRegister = () => router.push('/register')
</script>

<style scoped src="../styles/auth.css"></style>
