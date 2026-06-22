'use client'

import { useState } from 'react'
import Image from 'next/image'
import type { PhotoDto } from '@/types'

interface Props {
  photos: PhotoDto[]
  alt: string
}

export default function PhotoGallery({ photos, alt }: Props) {
  const [activeIndex, setActiveIndex] = useState(0)

  if (photos.length === 0) {
    return (
      <div className="relative w-full h-80 rounded-2xl overflow-hidden bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center">
        <p className="text-zinc-400 text-sm">Sin fotos disponibles</p>
      </div>
    )
  }

  return (
    <div className="space-y-3">
      <div className="relative w-full h-80 sm:h-[420px] rounded-2xl overflow-hidden bg-zinc-100 dark:bg-zinc-800">
        <Image
          key={photos[activeIndex].id}
          src={photos[activeIndex].url}
          alt={alt}
          fill
          className="object-cover"
          sizes="(max-width: 1280px) 100vw, 800px"
          priority
        />
      </div>

      {photos.length > 1 && (
        <div className="flex gap-2 overflow-x-auto pb-1">
          {photos.map((photo, i) => (
            <button
              key={photo.id}
              onClick={() => setActiveIndex(i)}
              className={`relative shrink-0 w-20 h-14 rounded-lg overflow-hidden border-2 transition-all ${
                i === activeIndex
                  ? 'border-red-500 opacity-100'
                  : 'border-transparent opacity-60 hover:opacity-90'
              }`}
            >
              <Image
                src={photo.url}
                alt={`Foto ${i + 1}`}
                fill
                className="object-cover"
                sizes="80px"
              />
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
