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
  // Валидация
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

<style scoped src="../styles/auth.css"></style>
