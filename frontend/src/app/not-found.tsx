import Link from 'next/link'
import { HouseSimple, MagnifyingGlass } from '@phosphor-icons/react/dist/ssr'

export default function NotFound() {
  return (
    <div className="min-h-[100dvh] bg-zinc-50 dark:bg-zinc-950 flex flex-col items-center justify-center px-4 text-center">
      <p className="text-[8rem] font-bold leading-none text-zinc-100 dark:text-zinc-900 select-none mb-2">
        404
      </p>
      <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-3">
        Página no encontrada
      </h1>
      <p className="text-zinc-500 dark:text-zinc-400 mb-8 max-w-sm text-sm leading-relaxed">
        La página que buscás no existe o fue movida. Probá buscando un alojamiento.
      </p>
      <div className="flex flex-col sm:flex-row gap-3">
        <Link
          href="/buscar"
          className="inline-flex items-center gap-2 bg-red-600 text-white px-6 py-3 rounded-xl font-semibold hover:bg-red-700 active:scale-[0.98] transition-all"
        >
          <MagnifyingGlass size={18} weight="bold" />
          Buscar alojamientos
        </Link>
        <Link
          href="/"
          className="inline-flex items-center gap-2 border border-zinc-200 dark:border-zinc-700 text-zinc-700 dark:text-zinc-300 px-6 py-3 rounded-xl font-semibold hover:bg-zinc-100 dark:hover:bg-zinc-800 active:scale-[0.98] transition-all"
        >
          <HouseSimple size={18} weight="fill" />
          Inicio
        </Link>
      </div>
    </div>
  )
}
