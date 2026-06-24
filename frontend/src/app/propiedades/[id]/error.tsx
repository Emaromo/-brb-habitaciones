'use client'

import { useEffect } from 'react'
import Link from 'next/link'
import { WarningCircle } from '@phosphor-icons/react'

export default function PropertyError({
  error,
  reset,
}: {
  error: Error & { digest?: string }
  reset: () => void
}) {
  useEffect(() => {
    console.error(error)
  }, [error])

  return (
    <div className="min-h-[60vh] flex flex-col items-center justify-center px-4 text-center">
      <WarningCircle size={44} className="text-red-400 mb-4" weight="fill" />
      <h2 className="text-lg font-bold text-zinc-900 dark:text-zinc-50 mb-2">
        No se pudo cargar la propiedad
      </h2>
      <p className="text-zinc-500 dark:text-zinc-400 mb-6 text-sm">
        Verificá tu conexión e intentá de nuevo.
      </p>
      <div className="flex gap-3">
        <button
          onClick={reset}
          className="px-5 py-2.5 bg-red-600 text-white rounded-xl text-sm font-semibold hover:bg-red-700 active:scale-[0.98] transition-all"
        >
          Reintentar
        </button>
        <Link
          href="/buscar"
          className="px-5 py-2.5 border border-zinc-200 dark:border-zinc-700 text-zinc-600 dark:text-zinc-300 rounded-xl text-sm font-semibold hover:bg-zinc-50 dark:hover:bg-zinc-800 transition-colors"
        >
          Ver otros alojamientos
        </Link>
      </div>
    </div>
  )
}
