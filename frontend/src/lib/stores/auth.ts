import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { UserDto } from '../api/auth'

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
