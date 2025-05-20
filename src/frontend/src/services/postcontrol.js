import axios from 'axios'


const api = axios.create({
  baseURL: '/api',
})


export const createPost = (text, code, jwt) => {
  return api.post(`/Post/Create?text=${text}&code=${code}`, null, jwt)
}

export const deletePost = (code, postId, jwt) => {
  return api.delete(`/Post/Delete?code=${code}&postId=${postId}`, jwt)
}

export const like = (code, postId, jwt) => {
  return api.post(`/Post/Like?code=${code}&postId=${postId}`, null, jwt)
}

export const disLike = (code, postId, jwt) => {
  return api.post(`/Post/DisLike?code=${code}&postId=${postId}`, null, jwt)
}

export const showPosts = (code, jwt) => {
  return api.get(`/Post/ShowPosts?code=${code}`, jwt)
}

export const showComments = (code, postId, jwt) => {
  return api.get(`/Post/ShowComments?code=${code}&postId=${postId}`, jwt)
}

export const createComment = (code, postId, text, jwt) => {
  return api.post(`/Post/CreateComment?code=${code}&postId=${postId}&text=${text}`, null, jwt)
}

export const deleteComment = (code, postId, commentId, jwt) => {
  return api.delete(`/Post/DeleteComment?code=${code}&postId=${postId}&commentId=${commentId}`, jwt)
}