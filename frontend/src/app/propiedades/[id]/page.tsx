import { notFound } from 'next/navigation'
import Image from 'next/image'
import Link from 'next/link'
import { MapPin, House } from '@phosphor-icons/react/dist/ssr'
import Navbar from '@/components/Navbar'
import RoomCard from '@/components/RoomCard'
import { serverFetch } from '@/lib/api/fetcher'
import type { PropertyDto } from '@/types'

export default async function PropiedadPage({ params }: { params: { id: string } }) {
  const property = await serverFetch<PropertyDto>(`/api/v1/properties/${params.id}`)

  if (!property) notFound()

  const coverPhoto = property.photos[0]?.url

  return (
    <div className="min-h-[100dvh] bg-zinc-50 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
      <Navbar />

      <main className="max-w-7xl mx-auto px-4 sm:px-6 py-8">
        {/* Cover */}
        <div className="relative w-full h-64 sm:h-96 rounded-2xl overflow-hidden bg-zinc-100 dark:bg-zinc-800 mb-8">
          {coverPhoto ? (
            <Image src={coverPhoto} alt={property.name} fill className="object-cover" priority sizes="100vw" />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-zinc-300">
              <House size={64} weight="thin" />
            </div>
          )}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-[1fr_300px] gap-10">
          {/* Left */}
          <div>
            <div className="flex items-start justify-between gap-4 mb-2">
              <h1 className="text-3xl font-bold text-zinc-900 dark:text-zinc-50">{property.name}</h1>
              <span className="shrink-0 bg-zinc-100 dark:bg-zinc-800 text-zinc-600 dark:text-zinc-400 text-xs font-medium px-3 py-1.5 rounded-full">
                {property.propertyType}
              </span>
            </div>

            <div className="flex items-center gap-1.5 text-sm text-zinc-500 dark:text-zinc-400 mb-6">
              <MapPin size={14} weight="fill" className="text-red-500" />
              <span>{property.address} — {property.city}, {property.province}</span>
            </div>

            <p className="text-zinc-600 dark:text-zinc-400 leading-relaxed mb-10">
              {property.description}
            </p>

            <h2 className="text-xl font-bold text-zinc-900 dark:text-zinc-50 mb-4">
              Habitaciones disponibles
            </h2>

            {property.rooms.length === 0 ? (
              <p className="text-zinc-400 text-sm">Esta propiedad aún no tiene habitaciones cargadas.</p>
            ) : (
              <div className="flex flex-col gap-3">
                {property.rooms.map(room => (
                  <RoomCard key={room.id} room={room} />
                ))}
              </div>
            )}
          </div>

          {/* Right sidebar */}
          <div>
            <div className="bg-white dark:bg-zinc-900 rounded-2xl border border-zinc-200 dark:border-zinc-800 p-5 sticky top-24">
              <p className="text-sm text-zinc-500 dark:text-zinc-400 mb-1">Publicado por</p>
              <p className="font-semibold text-zinc-900 dark:text-zinc-50">{property.ownerName}</p>

              {property.rooms.length > 0 && (
                <div className="mt-4 pt-4 border-t border-zinc-100 dark:border-zinc-800">
                  <p className="text-sm text-zinc-500 dark:text-zinc-400 mb-0.5">Desde</p>
                  <p className="text-2xl font-bold text-red-600 dark:text-red-400">
                    ${Math.min(...property.rooms.map(r => r.pricePerNight)).toLocaleString('es-AR')}
                    <span className="text-sm font-normal text-zinc-400">/noche</span>
                  </p>
                </div>
              )}

              <p className="text-xs text-zinc-400 mt-4">
                {property.rooms.length} {property.rooms.length === 1 ? 'habitación disponible' : 'habitaciones disponibles'}
              </p>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
