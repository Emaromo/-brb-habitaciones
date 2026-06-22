'use client'

import { useEffect, useState } from 'react'
import { adminApi } from '@/lib/api/admin'
import type { AdminUserDto } from '@/types'
import toast from 'react-hot-toast'
import { useAuthStore } from '@/lib/stores/auth'

const ROLES = ['Cliente', 'DuenoAlojamiento', 'Administrador']
const ROLE_LABELS: Record<string, string> = {
  Cliente: 'Cliente',
  DuenoAlojamiento: 'Dueño',
  Administrador: 'Admin',
}
const ROLE_CLS: Record<string, string> = {
  Cliente: 'bg-zinc-100 text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300',
  DuenoAlojamiento: 'bg-blue-100 text-blue-700 dark:bg-blue-900/30 dark:text-blue-400',
  Administrador: 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400',
}

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export default function AdminUsuariosPage() {
  const { user: me } = useAuthStore()
  const [users, setUsers] = useState<AdminUserDto[]>([])
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState<string | null>(null)
  const [filter, setFilter] = useState('')

  useEffect(() => {
    adminApi
      .getUsers()
      .then(setUsers)
      .catch(() => toast.error('No se pudieron cargar los usuarios.'))
      .finally(() => setLoading(false))
  }, [])

  const handleRoleChange = async (userId: string, newRole: string) => {
    if (userId === me?.id) { toast.error('No podés cambiar tu propio rol.'); return }
    setBusy(userId + ':role')
    try {
      const updated = await adminApi.changeRole(userId, newRole)
      setUsers(prev => prev.map(u => (u.id === userId ? updated : u)))
      toast.success('Rol actualizado.')
    } catch {
      toast.error('No se pudo actualizar el rol.')
    } finally {
      setBusy(null)
    }
  }

  const handleToggleActive = async (userId: string) => {
    if (userId === me?.id) { toast.error('No podés desactivarte a vos mismo.'); return }
    setBusy(userId + ':active')
    try {
      const updated = await adminApi.toggleActive(userId)
      setUsers(prev => prev.map(u => (u.id === userId ? updated : u)))
      toast.success(updated.isActive ? 'Usuario reactivado.' : 'Usuario desactivado.')
    } catch {
      toast.error('No se pudo cambiar el estado del usuario.')
    } finally {
      setBusy(null)
    }
  }

  const filtered = users.filter(u =>
    `${u.firstName} ${u.lastName} ${u.email}`.toLowerCase().includes(filter.toLowerCase())
  )

  return (
    <div>
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-6">Usuarios</h1>

      <div className="mb-4">
        <input
          type="search"
          placeholder="Buscar por nombre o email..."
          value={filter}
          onChange={e => setFilter(e.target.value)}
          className="w-full max-w-sm px-4 py-2 bg-white dark:bg-zinc-900 border border-zinc-300 dark:border-zinc-700 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-red-500"
        />
      </div>

      <div className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 overflow-hidden">
        {loading ? (
          <div className="p-4 space-y-3">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="h-12 rounded-lg bg-zinc-100 dark:bg-zinc-800 animate-pulse" />
            ))}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-zinc-100 dark:border-zinc-800">
                  <th className="text-left px-5 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Usuario</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Rol</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Estado</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Registrado</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-zinc-100 dark:divide-zinc-800">
                {filtered.map(u => (
                  <tr key={u.id} className={!u.isActive ? 'opacity-50' : ''}>
                    <td className="px-5 py-3.5">
                      <p className="font-medium text-zinc-900 dark:text-zinc-50">{u.firstName} {u.lastName}</p>
                      <p className="text-xs text-zinc-400">{u.email}</p>
                    </td>
                    <td className="px-4 py-3.5">
                      <select
                        value={u.role}
                        disabled={busy === u.id + ':role' || u.id === me?.id}
                        onChange={e => handleRoleChange(u.id, e.target.value)}
                        className={`text-xs font-semibold px-2.5 py-1 rounded-full border-0 cursor-pointer focus:outline-none focus:ring-2 focus:ring-red-500 disabled:cursor-not-allowed ${ROLE_CLS[u.role] ?? ROLE_CLS.Cliente}`}
                      >
                        {ROLES.map(r => (
                          <option key={r} value={r}>{ROLE_LABELS[r]}</option>
                        ))}
                      </select>
                    </td>
                    <td className="px-4 py-3.5">
                      <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${u.isActive ? 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400' : 'bg-zinc-100 text-zinc-400 dark:bg-zinc-800'}`}>
                        {u.isActive ? 'Activo' : 'Inactivo'}
                      </span>
                    </td>
                    <td className="px-4 py-3.5 text-xs text-zinc-400">{formatDate(u.createdAt)}</td>
                    <td className="px-4 py-3.5 text-right">
                      <button
                        onClick={() => handleToggleActive(u.id)}
                        disabled={busy === u.id + ':active' || u.id === me?.id}
                        className="text-xs text-zinc-400 hover:text-red-500 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
                      >
                        {busy === u.id + ':active' ? '...' : u.isActive ? 'Desactivar' : 'Reactivar'}
                      </button>
                    </td>
                  </tr>
                ))}
                {filtered.length === 0 && (
                  <tr>
                    <td colSpan={5} className="px-5 py-10 text-center text-sm text-zinc-400">
                      No hay usuarios que coincidan con la búsqueda.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
