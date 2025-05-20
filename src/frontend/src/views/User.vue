<template>
  <div class="user-container">
    <div class="profile-card">
      <h2>Профиль пользователя</h2>
      <div v-if="user">
        <p><strong>Имя:</strong> {{ user.name }}</p>
        <p><strong>Фамилия:</strong> {{ user.surname }}</p>
        <p><strong>Email:</strong> {{ user.email }}</p>
      </div>
      <div v-else class="loading">Загрузка профиля...</div>
    </div>

    <div class="chat-placeholder">
      <h3>Чат с {{ user?.name || '...' }}</h3>
      <div class="chat-box">
        <div class="messages" ref="messagesContainer">
          <div
            v-for="(msg, i) in messages"
            :key="i"
            class="message"
            :class="{ 'own': msg.senderEmail === currentEmail }"
          >
            <span>
              <strong>{{ msg.senderEmail === currentEmail ? 'Вы' : msg.senderEmail || 'Неизвестный' }}</strong>:
              {{ msg.text }}
            </span>
            <span class="timestamp">{{ new Date(msg.sentAt).toLocaleTimeString() }}</span>
          </div>
          <div ref="bottomAnchor" />
        </div>
        <div class="input-area">
          <input
            v-model="newMessage"
            @keyup.enter="sendMessage"
            placeholder="Введите сообщение..."
          />
          <button @click="sendMessage">Отправить</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onBeforeUnmount, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { defineProps } from 'vue'
import { showProfile, showMessages } from '../services/usercontrol'
import { HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'

const props = defineProps({
  email: { type: String, required: true }
})

const token = localStorage.getItem('token')

function parseJwt(token) {
  if (!token) return null
  try {
    return JSON.parse(atob(token.split('.')[1]))
  } catch (e) {
    return null
  }
}

const currentEmail = parseJwt(token)?.idemail
const router = useRouter()
const user = ref(null)
const messages = ref([])
const newMessage = ref('')
const connectionState = ref('Disconnected')

const messagesContainer = ref(null)
const bottomAnchor = ref(null)

function scrollToBottom() {
  nextTick(() => {
    bottomAnchor.value?.scrollIntoView({ behavior: 'smooth' })
  })
}

let connection = null

onMounted(async () => {
  if (props.email === currentEmail) {
    router.push('/workspace')
    return
  }

  try {
    const response = await showProfile({ params: { email: props.email } })
    user.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки профиля:', error)
  }

  try {
    const res = await showMessages(props.email, {
      headers: { Authorization: `Bearer ${token}` }
    })
    messages.value = res.data
    scrollToBottom()
  } catch (err) {
    console.error('Ошибка загрузки сообщений:', err)
  }

  connection = new HubConnectionBuilder()
    .withUrl('/api/chathub', {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .build()

  connection.on('ReceiveMessage', msg => {
   console.log(msg);
    messages.value.push(msg)
    scrollToBottom()
  })

  try {
    await connection.start()
    connectionState.value = connection.state
  } catch (err) {
    connectionState.value = 'Failed'
    console.error('Ошибка подключения к SignalR:', err)
  }
})

onBeforeUnmount(() => {
  if (connection) {
    connection.stop()
  }
})

async function sendMessage() {
  if (!newMessage.value.trim()) return

  if (!connection || connection.state !== HubConnectionState.Connected) {
    console.warn('Соединение с SignalR не установлено. Статус:', connection?.state)
    return
  }

  const messageToSend = newMessage.value.trim()

  try {
    await connection.invoke('SendMessage', props.email, messageToSend)
    newMessage.value = ''
    scrollToBottom()
  } catch (err) {
    console.error('Ошибка при отправке:', err)
  }
}
</script>

<style scoped>
.user-container {
  max-width: 800px;
  margin: 40px auto;
  padding: 24px;
  font-family: 'Segoe UI', sans-serif;
  color: #1f2937;
  background-color: #f9fafb;
}

.profile-card {
  background-color: #ffffff;
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  padding: 24px;
  margin-bottom: 32px;
}

.chat-placeholder {
  background-color: #ffffff;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.chat-box {
  display: flex;
  flex-direction: column;
  border: 1px solid #cbd5e1;
  border-radius: 12px;
  background-color: #f1f5f9;
  padding: 16px;
  max-height: 400px;
  overflow-y: auto;
}

.messages {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 16px;
}

.message {
  background-color: #e0e7ff;
  padding: 8px 12px;
  border-radius: 8px;
  align-self: flex-start;
  max-width: 80%;
  word-break: break-word;
}

.message.own {
  background-color: #c7f9cc;
  align-self: flex-end;
}

.timestamp {
  display: block;
  font-size: 0.75rem;
  color: #6b7280;
  margin-top: 4px;
}

.input-area {
  display: flex;
  gap: 8px;
}

input {
  flex: 1;
  padding: 8px;
  border-radius: 8px;
  border: 1px solid #cbd5e1;
}

button {
  padding: 8px 16px;
  background-color: #3f51b5;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
}
</style>
