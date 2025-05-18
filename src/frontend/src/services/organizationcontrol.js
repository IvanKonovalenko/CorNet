import axios from 'axios'


const api = axios.create({
  baseURL: 'http://localhost:5207',
})


export const create = (userData, jwt) => {
  return api.post('/Organization/Create', userData, jwt)
}


export const sendRequest = (jwt,joinCode) => {
  return api.post(`Organization/SendRequest?code=${joinCode}`, null, jwt)
}

export const showRequests = (code, jwt) => {
  return api.get(`/Organization/ShowRequests?code=${code}`, jwt)
}


export const acceptRequest = (organizationRequestId,code, jwt) => {
  return api.post(`Organization/AcceptRequest?OrganizationRequestId=${organizationRequestId}&code=${code}`, null, jwt)
}

export const refuseRequest = (organizationRequestId,code, jwt) => {
  return api.post(`Organization/RefuseRequest?OrganizationRequestId=${organizationRequestId}&code=${code}`, null, jwt)
}

export const showUsers = (code, jwt) => {
  return api.get(`/Organization/ShowUsers?code=${code}`, jwt)
}

export const deleteUser = (code, emailDeleteUser, jwt) => {
  return api.delete(`/Organization/DeleteUser?code=${code}&emailDeleteUser=${emailDeleteUser}`, jwt)
}

export const changeRoleUser = (code, emailRoleUser, role, jwt) => {
  return api.post(`/Organization/ChangeRoleUser?code=${code}&emailRoleUser=${emailRoleUser}&role=${role}`, null, jwt)
}
