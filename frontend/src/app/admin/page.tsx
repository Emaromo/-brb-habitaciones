'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import { adminApi } from '@/lib/api/admin'
import type { AdminStatsDto } from '@/types'
import { Users, Buildings, CalendarBlank, CurrencyDollar, ArrowRight, Warning } from '@phosphor-icons/react'
import toast from 'react-hot-toast'

function StatCard({
  label,
  value,
  icon: Icon,
  sub,
  accent = false,
}: {
  label: string
  value: string | number
  icon: React.ElementType
  sub?: string
  accent?: boolean
}) {
  return (
    <div className={`rounded-xl border p-5 ${accent ? 'border-red-200 dark:border-red-800 bg-red-50 dark:bg-red-900/20' : 'border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900'}`}>
      <div className="flex items-center justify-between mb-3">
        <p className="text-sm font-medium text-zinc-500 dark:text-zinc-400">{label}</p>
        <Icon size={18} className={accent ? 'text-red-500' : 'text-zinc-400'} />
      </div>
      <p className={`text-3xl font-bold ${accent ? 'text-red-600 dark:text-red-400' : 'text-zinc-900 dark:text-zinc-50'}`}>
        {value}
      </p>
      {sub && <p className="text-xs text-zinc-400 mt-1">{sub}</p>}
    </div>
  )
}

export default function AdminDashboard() {
  const [stats, setStats] = useState<AdminStatsDto | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    adminApi
      .getStats()
      .then(setStats)
      .catch(() => toast.error('No se pudieron cargar las estadísticas.'))
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return (
      <div>
        <div className="h-8 w-48 bg-zinc-200 dark:bg-zinc-700 rounded animate-pulse mb-6" />
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {[...Array(4)].map((_, i) => (
            <div key={i} className="h-28 rounded-xl bg-zinc-100 dark:bg-zinc-800 animate-pulse" />
          ))}
        </div>
      </div>
    )
  }

  if (!stats) return null

  return (
    <div>
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-6">
        Panel de Administración
      </h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatCard label="Usuarios registrados" value={stats.totalUsers} icon={Users} />
        <StatCard label="Propiedades" value={stats.totalProperties} icon={Buildings} sub={`${stats.totalRooms} habitaciones en total`} />
        <StatCard label="Reservas este mes" value={stats.reservationsThisMonth} icon={CalendarBlank} sub={`${stats.activeReservations} confirmadas activas`} />
        <StatCard
          label="Ingresos este mes"
          value={`$${stats.revenueThisMonth.toLocaleString('es-AR')}`}
          icon={CurrencyDollar}
        />
      </div>

      {stats.pendingProperties > 0 && (
        <Link
          href="/admin/propiedades"
          className="flex items-center gap-3 p-4 mb-6 rounded-xl border border-amber-200 dark:border-amber-800 bg-amber-50 dark:bg-amber-900/20 hover:bg-amber-100 dark:hover:bg-amber-900/30 transition-colors"
        >
          <Warning size={20} className="text-amber-500 shrink-0" weight="fill" />
          <div className="flex-1">
            <p className="font-semibold text-amber-800 dark:text-amber-300 text-sm">
              {stats.pendingProperties} {stats.pendingProperties === 1 ? 'propiedad pendiente' : 'propiedades pendientes'} de aprobación
            </p>
            <p className="text-xs text-amber-600 dark:text-amber-400">Revisalas antes de que aparezcan en los resultados de búsqueda</p>
          </div>
          <ArrowRight size={16} className="text-amber-500 shrink-0" />
        </Link>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        {[
          { href: '/admin/usuarios', label: 'Gestionar usuarios', icon: Users, desc: `${stats.totalUsers} registrados` },
          { href: '/admin/propiedades', label: 'Moderar propiedades', icon: Buildings, desc: `${stats.pendingProperties} pendientes` },
          { href: '/admin/reservas', label: 'Ver reservas', icon: CalendarBlank, desc: `${stats.activeReservations} activas` },
        ].map(({ href, label, icon: Icon, desc }) => (
          <Link
            key={href}
            href={href}
            className="flex items-center gap-4 p-5 rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:border-zinc-300 dark:hover:border-zinc-700 hover:shadow-sm transition-all group"
          >
            <div className="w-10 h-10 rounded-lg bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center group-hover:bg-red-50 dark:group-hover:bg-red-900/20 transition-colors">
              <Icon size={20} className="text-zinc-400 group-hover:text-red-500 transition-colors" />
            </div>
            <div>
              <p className="font-medium text-zinc-900 dark:text-zinc-50 text-sm">{label}</p>
              <p className="text-xs text-zinc-400">{desc}</p>
            </div>
            <ArrowRight size={14} className="ml-auto text-zinc-300 group-hover:text-zinc-500 transition-colors" />
          </Link>
        ))}
      </div>
    </div>
  )
}
