'use client'

import { useEffect } from 'react'
import { useRouter, usePathname } from 'next/navigation'
import Link from 'next/link'
import { useAuthStore, getRoleFromToken } from '@/lib/stores/auth'
import { ChartBar, Users, Buildings, CalendarBlank, SignOut } from '@phosphor-icons/react'

const navLinks = [
  { href: '/admin', label: 'Dashboard', icon: ChartBar, exact: true },
  { href: '/admin/usuarios', label: 'Usuarios', icon: Users },
  { href: '/admin/propiedades', label: 'Propiedades', icon: Buildings },
  { href: '/admin/reservas', label: 'Reservas', icon: CalendarBlank },
]

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, user, token, clearAuth } = useAuthStore()
  const router = useRouter()
  const pathname = usePathname()

  const role = getRoleFromToken(token) ?? user?.role ?? null
  const isAdmin = role === 'Administrador'

  useEffect(() => {
    if (!isAuthenticated) router.replace('/login')
    else if (!isAdmin) router.replace('/')
  }, [isAuthenticated, isAdmin, router])

  if (!isAuthenticated || !isAdmin) return null

  const handleLogout = () => {
    clearAuth()
    router.push('/')
  }

  return (
    <div className="min-h-[100dvh] flex bg-zinc-50 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
      {/* Sidebar */}
      <aside className="w-56 shrink-0 hidden md:flex flex-col border-r border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900">
        <div className="h-16 flex items-center px-5 border-b border-zinc-200 dark:border-zinc-800">
          <Link href="/" className="text-xl font-bold text-red-600 tracking-tight">
            BRB
          </Link>
          <span className="ml-2 text-xs font-semibold bg-red-100 text-red-600 dark:bg-red-900/30 dark:text-red-400 px-2 py-0.5 rounded-full">
            Admin
          </span>
        </div>

        <nav className="flex-1 p-3 space-y-1">
          {navLinks.map(({ href, label, icon: Icon, exact }) => {
            const active = exact ? pathname === href : pathname.startsWith(href)
            return (
              <Link
                key={href}
                href={href}
                className={`flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors ${
                  active
                    ? 'bg-red-50 dark:bg-red-900/20 text-red-600 dark:text-red-400'
                    : 'text-zinc-600 dark:text-zinc-400 hover:bg-zinc-100 dark:hover:bg-zinc-800 hover:text-zinc-900 dark:hover:text-zinc-100'
                }`}
              >
                <Icon size={18} weight={active ? 'fill' : 'regular'} />
                {label}
              </Link>
            )
          })}
        </nav>

        <div className="p-3 border-t border-zinc-200 dark:border-zinc-800">
          <div className="px-3 py-2 mb-1">
            <p className="text-xs font-medium text-zinc-700 dark:text-zinc-300 truncate">
              {user?.firstName} {user?.lastName}
            </p>
            <p className="text-xs text-zinc-400 truncate">{user?.email}</p>
          </div>
          <button
            onClick={handleLogout}
            className="flex items-center gap-2 w-full px-3 py-2 rounded-lg text-sm text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors"
          >
            <SignOut size={16} />
            Cerrar sesión
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Mobile header */}
        <header className="md:hidden h-14 flex items-center justify-between px-4 border-b border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900">
          <Link href="/" className="text-lg font-bold text-red-600">
            BRB <span className="text-xs font-semibold bg-red-100 text-red-600 px-2 py-0.5 rounded-full ml-1">Admin</span>
          </Link>
          <nav className="flex items-center gap-3">
            {navLinks.map(({ href, label, icon: Icon, exact }) => {
              const active = exact ? pathname === href : pathname.startsWith(href)
              return (
                <Link
                  key={href}
                  href={href}
                  title={label}
                  className={active ? 'text-red-600' : 'text-zinc-400'}
                >
                  <Icon size={20} weight={active ? 'fill' : 'regular'} />
                </Link>
              )
            })}
          </nav>
        </header>

        <main className="flex-1 p-6 overflow-auto">
          {children}
        </main>
      </div>
    </div>
  )
}
