import type { MetadataRoute } from 'next'
import type { PagedResult, PropertySummaryDto } from '@/types'

const BASE_URL = process.env.NEXT_PUBLIC_SITE_URL ?? 'https://brbhabitaciones.com.ar'
const API_URL = process.env.BACKEND_URL ?? 'http://localhost:8080'

async function fetchProperties(): Promise<PropertySummaryDto[]> {
  try {
    const res = await fetch(`${API_URL}/api/v1/properties?Page=1&PageSize=500`, {
      next: { revalidate: 3600 },
    })
    if (!res.ok) return []
    const json = await res.json()
    const data = json.data as PagedResult<PropertySummaryDto>
    return data?.items ?? []
  } catch {
    return []
  }
}

export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const staticRoutes: MetadataRoute.Sitemap = [
    { url: BASE_URL, lastModified: new Date(), changeFrequency: 'daily', priority: 1 },
    { url: `${BASE_URL}/buscar`, lastModified: new Date(), changeFrequency: 'daily', priority: 0.9 },
    { url: `${BASE_URL}/login`, lastModified: new Date(), changeFrequency: 'monthly', priority: 0.3 },
    { url: `${BASE_URL}/registro`, lastModified: new Date(), changeFrequency: 'monthly', priority: 0.3 },
  ]

  const properties = await fetchProperties()

  const propertyRoutes: MetadataRoute.Sitemap = properties.map(p => ({
    url: `${BASE_URL}/propiedades/${p.id}`,
    lastModified: new Date(p.createdAt),
    changeFrequency: 'weekly' as const,
    priority: 0.8,
  }))

  return [...staticRoutes, ...propertyRoutes]
}
