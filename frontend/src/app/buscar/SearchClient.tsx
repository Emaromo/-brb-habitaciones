'use client'

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { MagnifyingGlass, SlidersHorizontal } from '@phosphor-icons/react'
import { ARGENTINA_PROVINCES } from '@/lib/constants'

interface Props {
  defaultProvince?: string
  defaultCity?: string
  defaultMaxPrice?: string
  defaultMinCapacity?: string
}

export default function SearchClient({
  defaultProvince = '',
  defaultCity = '',
  defaultMaxPrice = '',
  defaultMinCapacity = '',
}: Props) {
  const router = useRouter()
  const [province, setProvince] = useState(defaultProvince)
  const [city, setCity] = useState(defaultCity)
  const [maxPrice, setMaxPrice] = useState(defaultMaxPrice)
  const [minCapacity, setMinCapacity] = useState(defaultMinCapacity)

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault()
    const params = new URLSearchParams()
    if (province) params.set('province', province)
    if (city) params.set('city', city)
    if (maxPrice) params.set('maxPrice', maxPrice)
    if (minCapacity) params.set('minCapacity', minCapacity)
    params.set('page', '1')
    router.push(`/buscar?${params.toString()}`)
  }

  const handleClear = () => {
    setProvince('')
    setCity('')
    setMaxPrice('')
    setMinCapacity('')
    router.push('/buscar')
  }

  const inputCls =
    'w-full px-3 py-2.5 bg-white dark:bg-zinc-800 border border-zinc-300 dark:border-zinc-600 rounded-lg text-sm text-zinc-900 dark:text-zinc-100 placeholder:text-zinc-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent'

  return (
    <form onSubmit={handleSearch} className="bg-white dark:bg-zinc-900 rounded-2xl border border-zinc-200 dark:border-zinc-800 p-5 shadow-sm">
      <div className="flex items-center gap-2 mb-4">
        <SlidersHorizontal size={16} className="text-zinc-500" weight="bold" />
        <span className="text-sm font-semibold text-zinc-700 dark:text-zinc-300">Filtros</span>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3">
        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Provincia
          </label>
          <select value={province} onChange={e => setProvince(e.target.value)} className={inputCls}>
            <option value="">Todas las provincias</option>
            {ARGENTINA_PROVINCES.map(p => (
              <option key={p} value={p}>{p}</option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Ciudad
          </label>
          <input
            type="text"
            value={city}
            onChange={e => setCity(e.target.value)}
            placeholder="Ej: Mar del Plata"
            className={inputCls}
          />
        </div>

        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Precio máx. por noche
          </label>
          <input
            type="number"
            value={maxPrice}
            onChange={e => setMaxPrice(e.target.value)}
            placeholder="$ sin límite"
            min={0}
            className={inputCls}
          />
        </div>

        <div>
          <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">
            Mín. huéspedes
          </label>
          <input
            type="number"
            value={minCapacity}
            onChange={e => setMinCapacity(e.target.value)}
            placeholder="1"
            min={1}
            max={20}
            className={inputCls}
          />
        </div>
      </div>

      <div className="flex items-center gap-3 mt-4">
        <button
          type="submit"
          className="flex items-center gap-2 px-5 py-2.5 bg-red-600 text-white text-sm font-semibold rounded-lg hover:bg-red-700 active:scale-[0.98] transition-all"
        >
          <MagnifyingGlass size={15} weight="bold" />
          Buscar
        </button>
        {(province || city || maxPrice || minCapacity) && (
          <button
            type="button"
            onClick={handleClear}
            className="text-sm text-zinc-500 dark:text-zinc-400 hover:text-zinc-700 dark:hover:text-zinc-200 transition-colors"
          >
            Limpiar filtros
          </button>
        )}
      </div>
    </form>
  )
}
