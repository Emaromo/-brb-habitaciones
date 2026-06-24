'use client'

import { useEffect } from 'react'
import Link from 'next/link'
import { WarningCircle } from '@phosphor-icons/react'

export default function GlobalError({
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
    <div className="min-h-[100dvh] bg-zinc-50 dark:bg-zinc-950 flex flex-col items-center justify-center px-4 text-center">
      <WarningCircle size={52} className="text-red-400 mb-4" weight="fill" />
      <h2 className="text-xl font-bold text-zinc-900 dark:text-zinc-50 mb-2">
        Algo salió mal
      </h2>
      <p className="text-zinc-500 dark:text-zinc-400 mb-8 max-w-sm text-sm leading-relaxed">
        Ocurrió un error inesperado. Podés intentarlo de nuevo o volver al inicio.
      </p>
      <div className="flex flex-col sm:flex-row gap-3">
        <button
          onClick={reset}
          className="px-6 py-3 bg-red-600 text-white rounded-xl font-semibold hover:bg-red-700 active:scale-[0.98] transition-all"
        >
          Reintentar
        </button>
        <Link
          href="/"
          className="px-6 py-3 border border-zinc-200 dark:border-zinc-700 text-zinc-700 dark:text-zinc-300 rounded-xl font-semibold hover:bg-zinc-100 dark:hover:bg-zinc-800 active:scale-[0.98] transition-all"
        >
          Ir al inicio
        </Link>
      </div>
    </div>
  )
}
