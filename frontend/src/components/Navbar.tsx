'use client'

import Link from 'next/link'
import { useAuthStore, getRoleFromToken } from '@/lib/stores/auth'
import { useRouter } from 'next/navigation'

export default function Navbar() {
  const { isAuthenticated, user, token, clearAuth } = useAuthStore()
  const router = useRouter()

  const role = getRoleFromToken(token) ?? user?.role ?? null
  const isAdmin = role === 'Administrador'

  const handleLogout = () => {
    clearAuth()
    router.push('/')
  }

  return (
    <header className="sticky top-0 z-40 h-16 bg-white/95 dark:bg-zinc-950/95 backdrop-blur-sm border-b border-zinc-200 dark:border-zinc-800">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 h-full flex items-center justify-between">
        <Link href="/" className="text-xl font-bold text-red-600 tracking-tight">
          BRB Habitaciones
        </Link>

        <nav className="hidden md:flex items-center gap-6 text-sm font-medium">
          {isAuthenticated ? (
            <>
              {isAdmin && (
                <Link
                  href="/admin"
                  className="text-red-600 dark:text-red-400 hover:text-red-700 dark:hover:text-red-300 font-semibold transition-colors"
                >
                  Admin
                </Link>
              )}
              <Link
                href="/dashboard"
                className="text-zinc-600 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors"
              >
                Mis propiedades
              </Link>
              <span className="text-zinc-400 text-xs">{user?.firstName}</span>
              <button
                onClick={handleLogout}
                className="text-zinc-600 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors"
              >
                Salir
              </button>
            </>
          ) : (
            <>
              <Link
                href="/dashboard"
                className="text-zinc-600 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors"
              >
                Publicar propiedad
              </Link>
              <Link
                href="/login"
                className="text-zinc-600 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors"
              >
                Iniciar sesión
              </Link>
              <Link
                href="/registro"
                className="bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 active:scale-[0.98] transition-all"
              >
                Registrarse
              </Link>
            </>
          )}
        </nav>

        <Link
          href={isAuthenticated ? '/dashboard' : '/registro'}
          className="md:hidden bg-red-600 text-white px-3 py-1.5 rounded-lg text-sm font-semibold"
        >
          {isAuthenticated ? 'Panel' : 'Registrarse'}
        </Link>
      </div>
    </header>
  )
}
