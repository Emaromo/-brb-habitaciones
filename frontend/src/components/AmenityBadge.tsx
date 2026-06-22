import { CheckCircle } from '@phosphor-icons/react/dist/ssr'
import type { AmenityDto } from '@/types'

export default function AmenityBadge({ amenity }: { amenity: AmenityDto }) {
  return (
    <span className="flex items-center gap-2 text-sm text-zinc-700 dark:text-zinc-300">
      <CheckCircle size={16} weight="fill" className="text-red-600 dark:text-red-400 shrink-0" />
      {amenity.name}
    </span>
  )
}
