import axios from 'axios'


const api = axios.create({
  baseURL: '/api',
})


export const login = (userData) => {
  return api.post('/Auth/Login', userData)
}


export const register = (userData) => {
  return api.post('/Auth/Register', userData)
}
