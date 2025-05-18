import axios from 'axios'


const api = axios.create({
  baseURL: 'http://localhost:5207',
})


export const login = (userData) => {
  return api.post('/Auth/Login', userData)
}


export const register = (userData) => {
  return api.post('/Auth/Register', userData)
}
