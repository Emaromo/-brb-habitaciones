'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { adminApi } from '@/lib/api/admin'
import type { AdminPropertyDto } from '@/types'
import toast from 'react-hot-toast'
import { CheckCircle, XCircle, ArrowSquareOut } from '@phosphor-icons/react'

type Tab = 'pendientes' | 'aprobadas' | 'rechazadas'

function formatDate(d: string) {
  return new Date(d).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export default function AdminPropiedadesPage() {
  const [properties, setProperties] = useState<AdminPropertyDto[]>([])
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState<string | null>(null)
  const [tab, setTab] = useState<Tab>('pendientes')

  useEffect(() => {
    adminApi
      .getProperties()
      .then(setProperties)
      .catch(() => toast.error('No se pudieron cargar las propiedades.'))
      .finally(() => setLoading(false))
  }, [])

  const handleApprove = async (id: string) => {
    setBusy(id)
    try {
      const updated = await adminApi.approveProperty(id)
      setProperties(prev => prev.map(p => (p.id === id ? updated : p)))
      toast.success('Propiedad aprobada.')
    } catch {
      toast.error('No se pudo aprobar la propiedad.')
    } finally {
      setBusy(null)
    }
  }

  const handleReject = async (id: string) => {
    if (!confirm('¿Rechazar esta propiedad? Se ocultará de los resultados.')) return
    setBusy(id)
    try {
      const updated = await adminApi.rejectProperty(id)
      setProperties(prev => prev.map(p => (p.id === id ? updated : p)))
      toast.success('Propiedad rechazada.')
    } catch {
      toast.error('No se pudo rechazar la propiedad.')
    } finally {
      setBusy(null)
    }
  }

  const tabs: { key: Tab; label: string }[] = [
    { key: 'pendientes', label: 'Pendientes' },
    { key: 'aprobadas', label: 'Aprobadas' },
    { key: 'rechazadas', label: 'Rechazadas' },
  ]

  const filtered = properties.filter(p => {
    if (tab === 'pendientes') return !p.isApproved
    if (tab === 'aprobadas') return p.isApproved && p.isActive
    return !p.isActive
  })

  const counts = {
    pendientes: properties.filter(p => !p.isApproved).length,
    aprobadas: properties.filter(p => p.isApproved && p.isActive).length,
    rechazadas: properties.filter(p => !p.isActive).length,
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-6">Propiedades</h1>

      {/* Tabs */}
      <div className="flex gap-1 p-1 bg-zinc-100 dark:bg-zinc-800 rounded-xl w-fit mb-6">
        {tabs.map(({ key, label }) => (
          <button
            key={key}
            onClick={() => setTab(key)}
            className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors flex items-center gap-2 ${
              tab === key
                ? 'bg-white dark:bg-zinc-700 text-zinc-900 dark:text-zinc-50 shadow-sm'
                : 'text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100'
            }`}
          >
            {label}
            <span className={`text-xs px-1.5 py-0.5 rounded-full ${key === 'pendientes' && counts.pendientes > 0 ? 'bg-amber-100 text-amber-700 dark:bg-amber-900/40 dark:text-amber-400' : 'bg-zinc-200 dark:bg-zinc-600 text-zinc-500'}`}>
              {counts[key]}
            </span>
          </button>
        ))}
      </div>

      <div className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 overflow-hidden">
        {loading ? (
          <div className="p-4 space-y-3">
            {[...Array(4)].map((_, i) => (
              <div key={i} className="h-14 rounded-lg bg-zinc-100 dark:bg-zinc-800 animate-pulse" />
            ))}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-zinc-100 dark:border-zinc-800">
                  <th className="text-left px-5 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Propiedad</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Dueño</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Tipo</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Hab.</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Publicada</th>
                  <th className="px-4 py-3" />
                </tr>
              </thead>
              <tbody className="divide-y divide-zinc-100 dark:divide-zinc-800">
                {filtered.map(p => (
                  <tr key={p.id}>
                    <td className="px-5 py-3.5">
                      <div className="flex items-center gap-2">
                        <div>
                          <p className="font-medium text-zinc-900 dark:text-zinc-50">{p.name}</p>
                          <p className="text-xs text-zinc-400">{p.city}, {p.province}</p>
                        </div>
                        <Link href={`/propiedades/${p.id}`} target="_blank" className="text-zinc-300 hover:text-zinc-500">
                          <ArrowSquareOut size={14} />
                        </Link>
                      </div>
                    </td>
                    <td className="px-4 py-3.5 text-zinc-600 dark:text-zinc-300">{p.ownerName}</td>
                    <td className="px-4 py-3.5 text-xs text-zinc-500">{p.propertyType}</td>
                    <td className="px-4 py-3.5 text-zinc-600 dark:text-zinc-300">{p.roomCount}</td>
                    <td className="px-4 py-3.5 text-xs text-zinc-400">{formatDate(p.createdAt)}</td>
                    <td className="px-4 py-3.5">
                      <div className="flex items-center gap-2 justify-end">
                        {!p.isApproved ? (
                          <button
                            onClick={() => handleApprove(p.id)}
                            disabled={busy === p.id}
                            className="flex items-center gap-1.5 text-xs font-medium text-emerald-600 hover:text-emerald-700 disabled:opacity-50 transition-colors"
                          >
                            <CheckCircle size={15} />
                            {busy === p.id ? '...' : 'Aprobar'}
                          </button>
                        ) : (
                          <button
                            onClick={() => handleReject(p.id)}
                            disabled={busy === p.id}
                            className="flex items-center gap-1.5 text-xs font-medium text-zinc-400 hover:text-red-500 disabled:opacity-50 transition-colors"
                          >
                            <XCircle size={15} />
                            {busy === p.id ? '...' : 'Rechazar'}
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
                {filtered.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-5 py-10 text-center text-sm text-zinc-400">
                      {tab === 'pendientes' ? '¡Todo al día! No hay propiedades pendientes de revisión.' : 'No hay propiedades en esta categoría.'}
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
