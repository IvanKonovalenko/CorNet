<template>
  <div id="app">
    <div v-if="!token">
      <h1 class="main-title">Authentication</h1>
      <div class="auth-section">
        <div class="form-container">
          <h2>Register</h2>
          <form @submit.prevent="register" class="auth-form">
            <div class="form-group">
              <label>Email:</label>
              <input v-model="registerForm.email" type="email" required />
            </div>
            <div class="form-group">
              <label>Name:</label>
              <input v-model="registerForm.name" type="text" required />
            </div>
            <div class="form-group">
              <label>Surname:</label>
              <input v-model="registerForm.surname" type="text" required />
            </div>
            <div class="form-group">
              <label>Password:</label>
              <input v-model="registerForm.password" type="password" required />
            </div>
            <div class="form-group">
              <label>Organization Code:</label>
              <input v-model="registerForm.organizationCode" type="text" required />
            </div>
            <div class="form-group">
              <label>Role:</label>
              <select v-model="registerForm.role">
                <option :value="0">Chief</option>
                <option :value="1">Employee</option>
              </select>
            </div>
            <button type="submit" class="btn">Register</button>
          </form>
          <p v-if="registerError" class="error">{{ registerError }}</p>
        </div>

        <div class="form-container">
          <h2>Login</h2>
          <form @submit.prevent="login" class="auth-form">
            <div class="form-group">
              <label>Email:</label>
              <input v-model="loginForm.email" type="email" required />
            </div>
            <div class="form-group">
              <label>Password:</label>
              <input v-model="loginForm.password" type="password" required />
            </div>
            <button type="submit" class="btn">Login</button>
          </form>
          <p v-if="loginError" class="error">{{ loginError }}</p>
        </div>
      </div>
    </div>

    <div v-else>
      <h1>Dashboard</h1>
      <div class="content-section">
        <div class="list-container">
          <h2>Users</h2>
          <ul>
            <li v-for="user in users" :key="user.email">
              {{ user.name }} {{ user.surname }} ({{ user.email }}) - Role: {{ user.role === 0 ? 'Chief' : 'Employee' }}
            </li>
          </ul>
        </div>

        <div class="messages-container">
          <h2>Messages</h2>
          <ul>
            <li v-for="message in messages" :key="message.id">
              {{ message.sendedMessage }} (to: {{ message.emailRecipient }})
            </li>
          </ul>
          <form @submit.prevent="sendMessage">
            <div class="form-group">
              <label for="recipient">Recipient:</label>
              <select id="recipient" v-model="newMessage.emailRecipient" required>
                <option v-for="user in users" :key="user.email" :value="user.email">
                  {{ user.name }} {{ user.surname }} ({{ user.email }})
                </option>
              </select>
            </div>
            <div class="form-group">
              <label for="message">Message:</label>
              <input id="message" v-model="newMessage.sendedMessage" placeholder="Type your message" required />
            </div>
            <button type="submit" class="btn">Send</button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      token: null,
      registerForm: {
        email: "",
        name: "",
        surname: "",
        password: "",
        organizationCode: "",
        role: 0,
      },
      loginForm: {
        email: "",
        password: "",
      },
      registerError: "",
      loginError: "",
      users: [],
      messages: [],
      newMessage: {
        sendedMessage: "",
        emailRecipient: "",
      },
    };
  },
  methods: {
    async register() {
      try {
        this.registerError = "";
        await axios.post("https://localhost:7157/Auth/Register", this.registerForm);
        alert("Registration successful!");
      } catch (error) {
        this.registerError = error.response?.data?.error || "Registration failed.";
      }
    },
    async login() {
      try {
        this.loginError = "";
        const response = await axios.post("https://localhost:7157/Auth/Login", this.loginForm);
        this.token = response.data;
        localStorage.setItem("jwt", this.token);
        await this.fetchUsers();
        await this.fetchMessages();
      } catch (error) {
        this.loginError = error.response?.data?.error || "Login failed.";
      }
    },
    async fetchUsers() {
      try {
        const response = await axios.get("https://localhost:7157/Organization/Users", {
          headers: { Authorization: `Bearer ${this.token}` },
        });
        this.users = response.data;
      } catch (error) {
        console.error("Failed to fetch users", error);
      }
    },
    async fetchMessages() {
      try {
        const response = await axios.get("https://localhost:7157/Message/Messages", {
          headers: { Authorization: `Bearer ${this.token}` },
        });
        this.messages = response.data;
      } catch (error) {
        console.error("Failed to fetch messages", error);
      }
    },
    async sendMessage() {
      try {
        const response = await axios.post(
          "https://localhost:7157/Message/SendMessage",
          {
            sendedMessage: this.newMessage.sendedMessage,
            dateTime: new Date().toISOString(),
            emailRecipient: this.newMessage.emailRecipient,
          },
          { headers: { Authorization: `Bearer ${this.token}` } }
        );
        this.messages.push(response.data);
        this.newMessage.sendedMessage = "";
      } catch (error) {
        console.error("Failed to send message", error);
      }
    },
  },
  created() {
    const savedToken = localStorage.getItem("jwt");
    if (savedToken) {
      this.token = savedToken;
      this.fetchUsers();
      this.fetchMessages();
    }
  },
};
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  margin-top: 20px;
  background-color: #f5f5f5;
  padding: 20px;
  border-radius: 10px;
  max-width: 800px;
  margin: auto;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.content-section {
  display: flex;
  justify-content: space-between;
  gap: 20px;
}

.list-container, .messages-container {
  background: #fff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.auth-section {
  display: flex;
  gap: 20px;
}

.form-container {
  background: #fff;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  width: 100%;
}

.auth-form {
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 15px;
}

.btn {
  background-color: #4CAF50;
  color: white;
  border: none;
  padding: 10px;
  cursor: pointer;
  border-radius: 5px;
}

.btn:hover {
  background-color: #45a049;
}

.error {
  color: red;
}
</style>
