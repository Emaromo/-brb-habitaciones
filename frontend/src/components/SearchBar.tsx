'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { MagnifyingGlass } from '@phosphor-icons/react'

export default function SearchBar() {
  const [query, setQuery] = useState('')
  const router = useRouter()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (query.trim()) router.push(`/buscar?q=${encodeURIComponent(query.trim())}`)
  }

  return (
    <form onSubmit={handleSubmit} className="flex">
      <div className="relative flex-1">
        <MagnifyingGlass
          size={18}
          weight="bold"
          className="absolute left-3.5 top-1/2 -translate-y-1/2 text-zinc-400 pointer-events-none"
        />
        <input
          type="text"
          value={query}
          onChange={e => setQuery(e.target.value)}
          placeholder="¿A dónde vas? Provincia o ciudad..."
          aria-label="Buscar destino"
          className="w-full pl-10 pr-4 py-3.5 bg-white dark:bg-zinc-800 border border-zinc-300 dark:border-zinc-600 border-r-0 rounded-l-xl text-sm text-zinc-900 dark:text-zinc-100 placeholder:text-zinc-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
        />
      </div>
      <button
        type="submit"
        className="px-6 py-3.5 bg-red-600 text-white font-semibold text-sm rounded-r-xl hover:bg-red-700 active:scale-[0.98] transition-all whitespace-nowrap"
      >
        Buscar
      </button>
    </form>
  )
}
