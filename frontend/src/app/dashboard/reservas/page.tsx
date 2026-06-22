'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { useAuthStore } from '@/lib/stores/auth'
import { reservationsApi } from '@/lib/api/reservations'
import type { ReservationDto } from '@/types'
import toast from 'react-hot-toast'
import { CalendarBlank, MapPin, User, XCircle } from '@phosphor-icons/react'

const statusLabel: Record<string, string> = {
  Confirmada: 'Confirmada',
  Cancelada: 'Cancelada',
  Completada: 'Completada',
}

const statusCls: Record<string, string> = {
  Confirmada: 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/40 dark:text-emerald-400',
  Cancelada: 'bg-zinc-100 text-zinc-500 dark:bg-zinc-800 dark:text-zinc-400',
  Completada: 'bg-blue-100 text-blue-700 dark:bg-blue-900/40 dark:text-blue-400',
}

function formatDate(d: string) {
  return new Date(d + 'T12:00:00').toLocaleDateString('es-AR', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
  })
}

export default function ReservasPage() {
  const { user } = useAuthStore()
  const [reservations, setReservations] = useState<ReservationDto[]>([])
  const [loading, setLoading] = useState(true)
  const [cancelling, setCancelling] = useState<string | null>(null)

  const isOwner = user?.role === 'DuenoAlojamiento'

  useEffect(() => {
    reservationsApi
      .getMy()
      .then(setReservations)
      .catch(() => toast.error('No se pudieron cargar las reservas.'))
      .finally(() => setLoading(false))
  }, [])

  const handleCancel = async (id: string) => {
    if (!confirm('¿Cancelar esta reserva?')) return
    setCancelling(id)
    try {
      const updated = await reservationsApi.cancel(id)
      setReservations(prev => prev.map(r => (r.id === id ? updated : r)))
      toast.success('Reserva cancelada.')
    } catch (err: any) {
      toast.error(err?.response?.data?.message ?? 'No se pudo cancelar.')
    } finally {
      setCancelling(null)
    }
  }

  if (loading) {
    return (
      <div className="space-y-4">
        {[...Array(3)].map((_, i) => (
          <div key={i} className="h-28 rounded-xl bg-zinc-100 dark:bg-zinc-800 animate-pulse" />
        ))}
      </div>
    )
  }

  if (reservations.length === 0) {
    return (
      <div className="text-center py-20">
        <CalendarBlank size={48} className="text-zinc-300 dark:text-zinc-600 mx-auto mb-4" />
        <h2 className="text-lg font-semibold text-zinc-700 dark:text-zinc-200 mb-2">
          {isOwner ? 'Todavía no tenés reservas en tus alojamientos' : 'Todavía no hiciste ninguna reserva'}
        </h2>
        {!isOwner && (
          <Link
            href="/buscar"
            className="mt-4 inline-block bg-red-600 text-white px-6 py-2.5 rounded-xl text-sm font-semibold hover:bg-red-700 transition-colors"
          >
            Buscar alojamientos
          </Link>
        )}
      </div>
    )
  }

  return (
    <div>
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-6">
        {isOwner ? 'Reservas de mis alojamientos' : 'Mis reservas'}
      </h1>

      <div className="space-y-4">
        {reservations.map(r => (
          <div
            key={r.id}
            className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 p-5 flex flex-col sm:flex-row sm:items-center gap-4"
          >
            {/* Info */}
            <div className="flex-1 min-w-0">
              <div className="flex items-start gap-3 mb-2">
                <div>
                  <Link
                    href={`/habitaciones/${r.roomId}`}
                    className="font-semibold text-zinc-900 dark:text-zinc-50 hover:text-red-600 dark:hover:text-red-400 transition-colors"
                  >
                    {r.roomTitle}
                  </Link>
                  <div className="flex flex-wrap items-center gap-x-3 gap-y-1 text-sm text-zinc-500 dark:text-zinc-400 mt-0.5">
                    <span className="flex items-center gap-1">
                      <MapPin size={12} weight="fill" className="text-red-500" />
                      {r.propertyName} · {r.propertyCity}
                    </span>
                    {isOwner && (
                      <span className="flex items-center gap-1">
                        <User size={12} />
                        {r.clientName}
                      </span>
                    )}
                  </div>
                </div>
              </div>

              <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-zinc-600 dark:text-zinc-400">
                <span className="flex items-center gap-1.5">
                  <CalendarBlank size={14} />
                  {formatDate(r.checkInDate)} → {formatDate(r.checkOutDate)}
                </span>
                <span>
                  {r.nights} {r.nights === 1 ? 'noche' : 'noches'} · {r.guestCount}{' '}
                  {r.guestCount === 1 ? 'huésped' : 'huéspedes'}
                </span>
              </div>

              {r.cancellationReason && (
                <p className="text-xs text-zinc-400 mt-1.5">Motivo: {r.cancellationReason}</p>
              )}
            </div>

            {/* Right: price + status + actions */}
            <div className="flex sm:flex-col items-center sm:items-end gap-3 shrink-0">
              <p className="font-bold text-lg text-zinc-900 dark:text-zinc-50">
                ${r.totalPrice.toLocaleString('es-AR')}
              </p>
              <span className={`px-3 py-1 rounded-full text-xs font-semibold ${statusCls[r.status] ?? statusCls.Confirmada}`}>
                {statusLabel[r.status] ?? r.status}
              </span>
              {r.status === 'Confirmada' && (
                <button
                  onClick={() => handleCancel(r.id)}
                  disabled={cancelling === r.id}
                  className="flex items-center gap-1.5 text-xs text-zinc-400 hover:text-red-500 transition-colors disabled:opacity-50"
                >
                  <XCircle size={14} />
                  {cancelling === r.id ? 'Cancelando...' : 'Cancelar'}
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
