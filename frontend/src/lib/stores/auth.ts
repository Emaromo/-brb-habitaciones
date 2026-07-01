import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { UserDto } from '../api/auth'

export function getRoleFromToken(token: string | null): string | null {
  if (!token) return null
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    return (
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      payload.role ??
      null
    )
  } catch {
    return null
  }
}

interface AuthState {
  token: string | null
  user: UserDto | null
  isAuthenticated: boolean
  setAuth: (token: string, user: UserDto) => void
  clearAuth: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    set => ({
      token: null,
      user: null,
      isAuthenticated: false,
      setAuth: (token, user) => set({ token, user, isAuthenticated: true }),
      clearAuth: () => set({ token: null, user: null, isAuthenticated: false }),
    }),
    { name: 'brb-auth' }
  )
)
