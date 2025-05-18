import axios from 'axios'


const api = axios.create({
  baseURL: 'http://localhost:5207', 
})

export const showProfile = (userData) => {
  return api.get('/User/ShowProfile', userData)
}


export const showOrganizations = (userData) => {
  return api.get('/User/ShowOrganizations', userData)
}
