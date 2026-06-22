import { Suspense } from 'react'
import Navbar from '@/components/Navbar'
import PropertyCard from '@/components/PropertyCard'
import SearchClient from './SearchClient'
import { serverFetch } from '@/lib/api/fetcher'
import type { PagedResult, PropertySummaryDto } from '@/types'

interface SearchParams {
  province?: string
  city?: string
  q?: string
  maxPrice?: string
  minCapacity?: string
  page?: string
}

async function fetchResults(sp: SearchParams) {
  const params = new URLSearchParams()
  if (sp.province) params.set('Province', sp.province)
  const cityOrQ = sp.city || sp.q
  if (cityOrQ) params.set('City', cityOrQ)
  if (sp.maxPrice) params.set('MaxPrice', sp.maxPrice)
  if (sp.minCapacity) params.set('MinCapacity', sp.minCapacity)
  params.set('Page', sp.page || '1')
  params.set('PageSize', '12')

  return serverFetch<PagedResult<PropertySummaryDto>>(
    `/api/v1/properties?${params.toString()}`
  )
}

export default async function BuscarPage({
  searchParams,
}: {
  searchParams: SearchParams
}) {
  const result = await fetchResults(searchParams)
  const items = result?.items ?? []
  const totalCount = result?.totalCount ?? 0
  const totalPages = result?.totalPages ?? 1
  const currentPage = Number(searchParams.page ?? 1)

  const buildPageUrl = (page: number) => {
    const params = new URLSearchParams()
    if (searchParams.province) params.set('province', searchParams.province)
    if (searchParams.city) params.set('city', searchParams.city)
    if (searchParams.q) params.set('q', searchParams.q)
    if (searchParams.maxPrice) params.set('maxPrice', searchParams.maxPrice)
    if (searchParams.minCapacity) params.set('minCapacity', searchParams.minCapacity)
    params.set('page', String(page))
    return `/buscar?${params.toString()}`
  }

  return (
    <div className="min-h-[100dvh] bg-zinc-50 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
      <Navbar />

      <main className="max-w-7xl mx-auto px-4 sm:px-6 py-8">
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-1">
            {searchParams.province || searchParams.city || searchParams.q
              ? `Resultados en ${searchParams.city || searchParams.q || searchParams.province}`
              : 'Todos los alojamientos'}
          </h1>
          <p className="text-sm text-zinc-500 dark:text-zinc-400">
            {totalCount} {totalCount === 1 ? 'propiedad encontrada' : 'propiedades encontradas'}
          </p>
        </div>

        <Suspense>
          <SearchClient
            defaultProvince={searchParams.province}
            defaultCity={searchParams.city || searchParams.q}
            defaultMaxPrice={searchParams.maxPrice}
            defaultMinCapacity={searchParams.minCapacity}
          />
        </Suspense>

        <div className="mt-8">
          {items.length === 0 ? (
            <div className="text-center py-20">
              <p className="text-zinc-400 text-lg mb-2">No se encontraron propiedades</p>
              <p className="text-zinc-500 text-sm">Probá con otros filtros o buscá en otra provincia</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
              {items.map(property => (
                <PropertyCard key={property.id} property={property} />
              ))}
            </div>
          )}
        </div>

        {totalPages > 1 && (
          <div className="flex items-center justify-center gap-2 mt-10">
            {currentPage > 1 && (
              <a
                href={buildPageUrl(currentPage - 1)}
                className="px-4 py-2 text-sm border border-zinc-300 dark:border-zinc-700 rounded-lg hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors"
              >
                Anterior
              </a>
            )}
            <span className="text-sm text-zinc-500 px-2">
              Página {currentPage} de {totalPages}
            </span>
            {currentPage < totalPages && (
              <a
                href={buildPageUrl(currentPage + 1)}
                className="px-4 py-2 text-sm border border-zinc-300 dark:border-zinc-700 rounded-lg hover:bg-zinc-100 dark:hover:bg-zinc-800 transition-colors"
              >
                Siguiente
              </a>
            )}
          </div>
        )}
      </main>
    </div>
  )
}
