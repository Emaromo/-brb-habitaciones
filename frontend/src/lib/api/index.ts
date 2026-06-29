import axios from 'axios'

export const apiClient = axios.create({
  headers: { 'Content-Type': 'application/json' },
})

apiClient.interceptors.request.use(config => {
  if (typeof window !== 'undefined') {
    const state = localStorage.getItem('brb-auth')
    if (state) {
      const { state: { token } } = JSON.parse(state)
      if (token) config.headers.Authorization = `Bearer ${token}`
    }
  }
  return config
})

apiClient.interceptors.response.use(
  res => res,
  err => {
    if (err.response?.status === 401 && typeof window !== 'undefined') {
      localStorage.removeItem('brb-auth')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)
