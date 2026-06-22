import Image from 'next/image'
import Link from 'next/link'
import { Bed, Users } from '@phosphor-icons/react/dist/ssr'
import type { RoomSummaryDto } from '@/types'

export default function RoomCard({ room }: { room: RoomSummaryDto }) {
  return (
    <Link
      href={`/habitaciones/${room.id}`}
      className="group flex gap-4 p-4 rounded-xl border border-zinc-200 dark:border-zinc-800 bg-white dark:bg-zinc-900 hover:border-red-300 dark:hover:border-red-800 hover:shadow-md transition-all"
    >
      <div className="relative w-32 h-24 shrink-0 rounded-lg overflow-hidden bg-zinc-100 dark:bg-zinc-800">
        {room.coverPhotoUrl ? (
          <Image
            src={room.coverPhotoUrl}
            alt={room.title}
            fill
            className="object-cover group-hover:scale-105 transition-transform duration-500"
            sizes="128px"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-zinc-300 dark:text-zinc-600">
            <Bed size={28} weight="thin" />
          </div>
        )}
      </div>

      <div className="flex flex-col justify-between min-w-0">
        <div>
          <h3 className="font-semibold text-zinc-900 dark:text-zinc-50 line-clamp-1">
            {room.title}
          </h3>
          <div className="flex items-center gap-1.5 mt-1 text-xs text-zinc-500 dark:text-zinc-400">
            <Users size={13} weight="bold" />
            <span>{room.capacity} {room.capacity === 1 ? 'huésped' : 'huéspedes'}</span>
          </div>
        </div>
        <span className="text-base font-semibold text-red-600 dark:text-red-400">
          ${room.pricePerNight.toLocaleString('es-AR')}
          <span className="text-xs font-normal text-zinc-400">/noche</span>
        </span>
      </div>
    </Link>
  )
}
