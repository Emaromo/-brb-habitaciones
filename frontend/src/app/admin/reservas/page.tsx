'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { adminApi } from '@/lib/api/admin'
import type { ReservationDto } from '@/types'
import toast from 'react-hot-toast'

const STATUS_CLS: Record<string, string> = {
  Confirmada: 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/40 dark:text-emerald-400',
  Cancelada: 'bg-zinc-100 text-zinc-500 dark:bg-zinc-800 dark:text-zinc-400',
  Completada: 'bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-400',
}

function formatDate(d: string) {
  return new Date(d + 'T12:00:00').toLocaleDateString('es-AR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  })
}

export default function AdminReservasPage() {
  const [reservations, setReservations] = useState<ReservationDto[]>([])
  const [loading, setLoading] = useState(true)
  const [filter, setFilter] = useState<string>('todas')

  useEffect(() => {
    adminApi
      .getReservations()
      .then(setReservations)
      .catch(() => toast.error('No se pudieron cargar las reservas.'))
      .finally(() => setLoading(false))
  }, [])

  const filtered = filter === 'todas'
    ? reservations
    : reservations.filter(r => r.status === filter)

  const counts = {
    todas: reservations.length,
    Confirmada: reservations.filter(r => r.status === 'Confirmada').length,
    Cancelada: reservations.filter(r => r.status === 'Cancelada').length,
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-6">Reservas</h1>

      {/* Filter pills */}
      <div className="flex gap-2 mb-6 flex-wrap">
        {[
          { key: 'todas', label: 'Todas' },
          { key: 'Confirmada', label: 'Confirmadas' },
          { key: 'Cancelada', label: 'Canceladas' },
        ].map(({ key, label }) => (
          <button
            key={key}
            onClick={() => setFilter(key)}
            className={`px-4 py-1.5 rounded-full text-sm font-medium transition-colors border ${
              filter === key
                ? 'bg-red-600 text-white border-red-600'
                : 'bg-white dark:bg-zinc-900 text-zinc-600 dark:text-zinc-300 border-zinc-200 dark:border-zinc-700 hover:border-zinc-400'
            }`}
          >
            {label} <span className="ml-1 opacity-70">{counts[key as keyof typeof counts] ?? filtered.length}</span>
          </button>
        ))}
      </div>

      <div className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 overflow-hidden">
        {loading ? (
          <div className="p-4 space-y-3">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="h-14 rounded-lg bg-zinc-100 dark:bg-zinc-800 animate-pulse" />
            ))}
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-zinc-100 dark:border-zinc-800">
                  <th className="text-left px-5 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Cliente</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Habitación</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Fechas</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Noches</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Total</th>
                  <th className="text-left px-4 py-3 text-xs font-semibold text-zinc-400 uppercase tracking-wider">Estado</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-zinc-100 dark:divide-zinc-800">
                {filtered.map(r => (
                  <tr key={r.id}>
                    <td className="px-5 py-3.5">
                      <p className="font-medium text-zinc-900 dark:text-zinc-50">{r.clientName}</p>
                    </td>
                    <td className="px-4 py-3.5">
                      <Link
                        href={`/habitaciones/${r.roomId}`}
                        target="_blank"
                        className="text-zinc-700 dark:text-zinc-300 hover:text-red-600 dark:hover:text-red-400 transition-colors"
                      >
                        {r.roomTitle}
                      </Link>
                      <p className="text-xs text-zinc-400">{r.propertyName} · {r.propertyCity}</p>
                    </td>
                    <td className="px-4 py-3.5 text-zinc-600 dark:text-zinc-300 text-xs">
                      {formatDate(r.checkInDate)} → {formatDate(r.checkOutDate)}
                    </td>
                    <td className="px-4 py-3.5 text-zinc-600 dark:text-zinc-300">
                      {r.nights}
                    </td>
                    <td className="px-4 py-3.5 font-semibold text-zinc-900 dark:text-zinc-50">
                      ${r.totalPrice.toLocaleString('es-AR')}
                    </td>
                    <td className="px-4 py-3.5">
                      <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${STATUS_CLS[r.status] ?? STATUS_CLS.Confirmada}`}>
                        {r.status}
                      </span>
                    </td>
                  </tr>
                ))}
                {filtered.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-5 py-10 text-center text-sm text-zinc-400">
                      No hay reservas en esta categoría.
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
