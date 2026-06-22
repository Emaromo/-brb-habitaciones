'use client'

import { useEffect, useState } from 'react'
import Link from 'next/link'
import Image from 'next/image'
import { Plus, House, PencilSimple, Trash } from '@phosphor-icons/react'
import { propertiesApi } from '@/lib/api/properties'
import type { PropertySummaryDto } from '@/types'
import toast from 'react-hot-toast'

export default function DashboardPage() {
  const [properties, setProperties] = useState<PropertySummaryDto[]>([])
  const [loading, setLoading] = useState(true)

  const load = () => {
    propertiesApi
      .getMyProperties()
      .then(setProperties)
      .catch(() => toast.error('No se pudieron cargar las propiedades'))
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const handleDelete = async (id: string, name: string) => {
    if (!confirm(`¿Eliminar "${name}"? Esta acción no se puede deshacer.`)) return
    await toast.promise(propertiesApi.delete(id), {
      loading: 'Eliminando...',
      success: 'Propiedad eliminada',
      error: 'No se pudo eliminar',
    })
    load()
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">Mis propiedades</h1>
          <p className="text-sm text-zinc-500 dark:text-zinc-400 mt-0.5">
            Gestioná tus alojamientos y habitaciones
          </p>
        </div>
        <Link
          href="/dashboard/propiedades/nueva"
          className="flex items-center gap-2 bg-red-600 text-white px-4 py-2.5 rounded-lg text-sm font-semibold hover:bg-red-700 active:scale-[0.98] transition-all"
        >
          <Plus size={16} weight="bold" />
          Nueva propiedad
        </Link>
      </div>

      {loading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
          {[...Array(3)].map((_, i) => (
            <div key={i} className="h-64 rounded-xl bg-zinc-200 dark:bg-zinc-800 animate-pulse" />
          ))}
        </div>
      ) : properties.length === 0 ? (
        <div className="text-center py-20 bg-white dark:bg-zinc-900 rounded-2xl border border-zinc-200 dark:border-zinc-800">
          <House size={48} weight="thin" className="text-zinc-300 dark:text-zinc-600 mx-auto mb-4" />
          <p className="text-zinc-500 dark:text-zinc-400 mb-2">Todavía no publicaste ninguna propiedad</p>
          <Link
            href="/dashboard/propiedades/nueva"
            className="inline-flex items-center gap-1.5 text-sm text-red-600 dark:text-red-400 font-medium hover:underline"
          >
            <Plus size={14} weight="bold" />
            Publicar mi primera propiedad
          </Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
          {properties.map(p => (
            <div
              key={p.id}
              className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 overflow-hidden"
            >
              <div className="relative h-40 bg-zinc-100 dark:bg-zinc-800">
                {p.coverPhotoUrl ? (
                  <Image src={p.coverPhotoUrl} alt={p.name} fill className="object-cover" sizes="400px" />
                ) : (
                  <div className="w-full h-full flex items-center justify-center text-zinc-300 dark:text-zinc-600">
                    <House size={40} weight="thin" />
                  </div>
                )}
                <span className={`absolute top-2 right-2 text-xs px-2 py-0.5 rounded-full font-medium ${
                  p.roomCount > 0
                    ? 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/50 dark:text-emerald-300'
                    : 'bg-zinc-100 text-zinc-500 dark:bg-zinc-700 dark:text-zinc-400'
                }`}>
                  {p.roomCount} hab.
                </span>
              </div>

              <div className="p-4">
                <h3 className="font-semibold text-zinc-900 dark:text-zinc-50 line-clamp-1 mb-0.5">{p.name}</h3>
                <p className="text-xs text-zinc-500 dark:text-zinc-400">{p.city}, {p.province}</p>

                <div className="flex items-center gap-2 mt-4">
                  <Link
                    href={`/dashboard/propiedades/${p.id}`}
                    className="flex-1 flex items-center justify-center gap-1.5 py-2 text-sm font-medium border border-zinc-200 dark:border-zinc-700 rounded-lg hover:bg-zinc-50 dark:hover:bg-zinc-800 transition-colors"
                  >
                    <PencilSimple size={14} weight="bold" />
                    Gestionar
                  </Link>
                  <button
                    onClick={() => handleDelete(p.id, p.name)}
                    className="p-2 text-zinc-400 hover:text-red-600 dark:hover:text-red-400 border border-zinc-200 dark:border-zinc-700 rounded-lg hover:border-red-200 dark:hover:border-red-800 transition-colors"
                    title="Eliminar propiedad"
                  >
                    <Trash size={16} />
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
