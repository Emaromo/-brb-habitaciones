import Image from 'next/image'
import Link from 'next/link'
import { House } from '@phosphor-icons/react/dist/ssr'
import type { PropertySummaryDto } from '@/types'

export default function PropertyCard({ property }: { property: PropertySummaryDto }) {
  return (
    <Link
      href={`/propiedades/${property.id}`}
      className="group block rounded-xl overflow-hidden border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:border-red-300 dark:hover:border-red-800 hover:shadow-md transition-all"
    >
      <div className="relative h-48 bg-zinc-100 dark:bg-zinc-800">
        {property.coverPhotoUrl ? (
          <Image
            src={property.coverPhotoUrl}
            alt={property.name}
            fill
            className="object-cover group-hover:scale-105 transition-transform duration-500"
            sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-zinc-300 dark:text-zinc-600">
            <House size={48} weight="thin" />
          </div>
        )}
        <div className="absolute top-3 left-3">
          <span className="bg-white/90 dark:bg-zinc-900/90 text-zinc-700 dark:text-zinc-300 text-xs font-medium px-2.5 py-1 rounded-full border border-zinc-200 dark:border-zinc-700">
            {property.propertyType}
          </span>
        </div>
      </div>

      <div className="p-4">
        <h3 className="font-semibold text-zinc-900 dark:text-zinc-50 line-clamp-1 mb-0.5">
          {property.name}
        </h3>
        <p className="text-sm text-zinc-500 dark:text-zinc-400">
          {property.city}, {property.province}
        </p>
        <div className="flex items-center justify-between mt-3 pt-3 border-t border-zinc-100 dark:border-zinc-800">
          <span className="text-xs text-zinc-400 dark:text-zinc-500">
            {property.roomCount} {property.roomCount === 1 ? 'habitación' : 'habitaciones'}
          </span>
          {property.minPricePerNight != null && (
            <span className="text-sm font-semibold text-zinc-900 dark:text-zinc-50">
              desde ${property.minPricePerNight.toLocaleString('es-AR')}
              <span className="text-xs font-normal text-zinc-400">/noche</span>
            </span>
          )}
        </div>
      </div>
    </Link>
  )
}
