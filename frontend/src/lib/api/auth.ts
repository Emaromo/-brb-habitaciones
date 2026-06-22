import { apiClient } from './index'

export interface LoginPayload {
  email: string
  password: string
}

export interface RegisterPayload {
  email: string
  password: string
  firstName: string
  lastName: string
  phone?: string
}

export interface UserDto {
  id: string
  email: string
  firstName: string
  lastName: string
  role: string
}

export interface AuthData {
  accessToken: string
  refreshToken: string
  expiresAt: string
  user: UserDto
}

interface ApiResponse<T> {
  success: boolean
  data: T | null
  message: string | null
}

export const authApi = {
  login: (payload: LoginPayload) =>
    apiClient
      .post<ApiResponse<AuthData>>('/api/v1/auth/login', payload)
      .then(r => r.data.data!),

  register: (payload: RegisterPayload) =>
    apiClient
      .post<ApiResponse<AuthData>>('/api/v1/auth/register', payload)
      .then(r => r.data.data!),

  refresh: (refreshToken: string) =>
    apiClient
      .post<ApiResponse<AuthData>>('/api/v1/auth/refresh', { refreshToken })
      .then(r => r.data.data!),
}
