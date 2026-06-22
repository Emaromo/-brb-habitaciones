'use client'

import { useState, useMemo } from 'react'
import { useRouter } from 'next/navigation'
import { useAuthStore } from '@/lib/stores/auth'
import { reservationsApi } from '@/lib/api/reservations'
import Link from 'next/link'
import toast from 'react-hot-toast'

interface Props {
  roomId: string
  pricePerNight: number
  capacity: number
}

const today = new Date().toISOString().split('T')[0]

export default function BookingWidget({ roomId, pricePerNight, capacity }: Props) {
  const router = useRouter()
  const { isAuthenticated } = useAuthStore()
  const [checkIn, setCheckIn] = useState('')
  const [checkOut, setCheckOut] = useState('')
  const [guests, setGuests] = useState(1)
  const [loading, setLoading] = useState(false)

  const nights = useMemo(() => {
    if (!checkIn || !checkOut) return 0
    const diff = new Date(checkOut).getTime() - new Date(checkIn).getTime()
    return Math.max(0, Math.round(diff / 86_400_000))
  }, [checkIn, checkOut])

  const total = nights * pricePerNight

  const handleReserve = async () => {
    if (!checkIn || !checkOut || nights <= 0) {
      toast.error('Seleccioná las fechas de entrada y salida.')
      return
    }
    setLoading(true)
    try {
      await reservationsApi.create({
        roomId,
        checkInDate: checkIn,
        checkOutDate: checkOut,
        guestCount: guests,
      })
      toast.success('¡Reserva confirmada!')
      router.push('/dashboard/reservas')
    } catch (err: any) {
      const msg = err?.response?.data?.message ?? 'No se pudo confirmar la reserva.'
      toast.error(msg)
    } finally {
      setLoading(false)
    }
  }

  const inputCls =
    'w-full px-3 py-2.5 bg-white dark:bg-zinc-800 border border-zinc-300 dark:border-zinc-600 rounded-lg text-sm text-zinc-900 dark:text-zinc-100 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent'

  return (
    <div className="bg-white dark:bg-zinc-900 rounded-2xl border border-zinc-200 dark:border-zinc-800 p-6 shadow-sm">
      <p className="text-3xl font-bold text-zinc-900 dark:text-zinc-50 mb-1">
        ${pricePerNight.toLocaleString('es-AR')}
        <span className="text-base font-normal text-zinc-400">/noche</span>
      </p>
      <p className="text-sm text-zinc-500 dark:text-zinc-400 mb-5">
        Capacidad: {capacity} {capacity === 1 ? 'huésped' : 'huéspedes'}
      </p>

      <div className="space-y-3 mb-4">
        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Llegada
          </label>
          <input
            type="date"
            value={checkIn}
            min={today}
            onChange={e => {
              setCheckIn(e.target.value)
              if (checkOut && e.target.value >= checkOut) setCheckOut('')
            }}
            className={inputCls}
          />
        </div>

        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Salida
          </label>
          <input
            type="date"
            value={checkOut}
            min={checkIn || today}
            onChange={e => setCheckOut(e.target.value)}
            className={inputCls}
          />
        </div>

        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Huéspedes
          </label>
          <select
            value={guests}
            onChange={e => setGuests(Number(e.target.value))}
            className={inputCls}
          >
            {Array.from({ length: capacity }, (_, i) => i + 1).map(n => (
              <option key={n} value={n}>
                {n} {n === 1 ? 'huésped' : 'huéspedes'}
              </option>
            ))}
          </select>
        </div>
      </div>

      {nights > 0 && (
        <div className="flex items-center justify-between text-sm py-3 border-t border-zinc-100 dark:border-zinc-800 mb-4">
          <span className="text-zinc-500 dark:text-zinc-400">
            ${pricePerNight.toLocaleString('es-AR')} × {nights} {nights === 1 ? 'noche' : 'noches'}
          </span>
          <span className="font-semibold text-zinc-900 dark:text-zinc-50">
            ${total.toLocaleString('es-AR')}
          </span>
        </div>
      )}

      {isAuthenticated ? (
        <button
          onClick={handleReserve}
          disabled={loading || nights <= 0}
          className="w-full bg-red-600 text-white py-3.5 rounded-xl font-semibold text-base hover:bg-red-700 disabled:opacity-50 disabled:cursor-not-allowed active:scale-[0.98] transition-all"
        >
          {loading ? 'Confirmando...' : nights > 0 ? `Reservar — $${total.toLocaleString('es-AR')}` : 'Elegí las fechas'}
        </button>
      ) : (
        <div className="text-center">
          <p className="text-sm text-zinc-500 dark:text-zinc-400 mb-3">
            Para reservar necesitás una cuenta
          </p>
          <Link
            href="/login"
            className="block w-full bg-red-600 text-white py-3.5 rounded-xl font-semibold text-base hover:bg-red-700 active:scale-[0.98] transition-all text-center"
          >
            Iniciar sesión para reservar
          </Link>
        </div>
      )}
    </div>
  )
}
