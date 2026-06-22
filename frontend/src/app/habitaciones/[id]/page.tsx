import { notFound } from 'next/navigation'
import Link from 'next/link'
import { MapPin, Users, ArrowLeft } from '@phosphor-icons/react/dist/ssr'
import Navbar from '@/components/Navbar'
import PhotoGallery from '@/components/PhotoGallery'
import AmenityBadge from '@/components/AmenityBadge'
import BookingWidget from './BookingWidget'
import { serverFetch } from '@/lib/api/fetcher'
import type { RoomDto } from '@/types'

export default async function HabitacionPage({ params }: { params: { id: string } }) {
  const room = await serverFetch<RoomDto>(`/api/v1/rooms/${params.id}`)

  if (!room) notFound()

  const amenitiesByCategory = room.amenities.reduce<Record<string, typeof room.amenities>>(
    (acc, a) => {
      acc[a.category] = [...(acc[a.category] ?? []), a]
      return acc
    },
    {}
  )

  return (
    <div className="min-h-[100dvh] bg-zinc-50 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">
      <Navbar />

      <main className="max-w-5xl mx-auto px-4 sm:px-6 py-8">
        <Link
          href={`/propiedades/${room.propertyId}`}
          className="inline-flex items-center gap-1.5 text-sm text-zinc-500 dark:text-zinc-400 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors mb-6"
        >
          <ArrowLeft size={14} weight="bold" />
          {room.propertyName}
        </Link>

        <div className="grid grid-cols-1 lg:grid-cols-[1fr_320px] gap-10 items-start">
          {/* Left */}
          <div>
            <PhotoGallery photos={room.photos} alt={room.title} />

            <div className="mt-8">
              <h1 className="text-2xl sm:text-3xl font-bold text-zinc-900 dark:text-zinc-50 mb-2">
                {room.title}
              </h1>

              <div className="flex flex-wrap items-center gap-4 text-sm text-zinc-500 dark:text-zinc-400 mb-6">
                <span className="flex items-center gap-1.5">
                  <Users size={14} weight="bold" />
                  {room.capacity} {room.capacity === 1 ? 'huésped' : 'huéspedes'}
                </span>
                <span className="flex items-center gap-1.5">
                  <MapPin size={14} weight="fill" className="text-red-500" />
                  {room.propertyName}
                </span>
              </div>

              <p className="text-zinc-600 dark:text-zinc-400 leading-relaxed">
                {room.description}
              </p>

              {room.amenities.length > 0 && (
                <div className="mt-8">
                  <h2 className="text-lg font-semibold text-zinc-900 dark:text-zinc-50 mb-4">
                    Servicios incluidos
                  </h2>
                  <div className="space-y-4">
                    {Object.entries(amenitiesByCategory).map(([category, list]) => (
                      <div key={category}>
                        <p className="text-xs font-semibold text-zinc-400 dark:text-zinc-500 uppercase tracking-wider mb-2">
                          {category}
                        </p>
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-y-2">
                          {list.map(a => (
                            <AmenityBadge key={a.id} amenity={a} />
                          ))}
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Right: booking widget */}
          <div className="lg:sticky lg:top-24">
            <BookingWidget
              roomId={room.id}
              pricePerNight={room.pricePerNight}
              capacity={room.capacity}
            />
          </div>
        </div>
      </main>
    </div>
  )
}
