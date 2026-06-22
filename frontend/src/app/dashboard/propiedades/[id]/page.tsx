'use client'

import { useEffect, useRef, useState } from 'react'
import { useParams, useRouter } from 'next/navigation'
import Link from 'next/link'
import Image from 'next/image'
import { ArrowLeft, Plus, Trash, UploadSimple, Eye } from '@phosphor-icons/react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { propertiesApi } from '@/lib/api/properties'
import { roomsApi } from '@/lib/api/rooms'
import type { PropertyDto, RoomDto } from '@/types'
import toast from 'react-hot-toast'

const roomSchema = z.object({
  title: z.string().min(3, 'Mínimo 3 caracteres').max(200),
  description: z.string().min(10, 'Describí la habitación').max(2000),
  capacity: z.coerce.number().min(1, 'Mínimo 1 huésped').max(20),
  pricePerNight: z.coerce.number().min(1, 'Ingresá un precio válido'),
})

type RoomFormData = z.infer<typeof roomSchema>

export default function GestionarPropiedadPage() {
  const params = useParams<{ id: string }>()
  const router = useRouter()
  const [property, setProperty] = useState<PropertyDto | null>(null)
  const [loading, setLoading] = useState(true)
  const [showAddRoom, setShowAddRoom] = useState(false)
  const [uploadingFor, setUploadingFor] = useState<string | null>(null)
  const fileInputRef = useRef<HTMLInputElement>(null)

  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } =
    useForm<RoomFormData>({ resolver: zodResolver(roomSchema) })

  const load = async () => {
    try {
      const data = await propertiesApi.getById(params.id)
      setProperty(data)
    } catch {
      toast.error('No se pudo cargar la propiedad')
      router.push('/dashboard')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [params.id])

  const onAddRoom = async (data: RoomFormData) => {
    try {
      await roomsApi.create(params.id, data)
      toast.success('Habitación agregada')
      reset()
      setShowAddRoom(false)
      load()
    } catch {
      toast.error('No se pudo agregar la habitación')
    }
  }

  const handleDeleteRoom = async (roomId: string, title: string) => {
    if (!confirm(`¿Eliminar "${title}"?`)) return
    await toast.promise(roomsApi.delete(roomId), {
      loading: 'Eliminando...',
      success: 'Habitación eliminada',
      error: 'No se pudo eliminar',
    })
    load()
  }

  const handleUploadPhoto = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0]
    if (!file || !uploadingFor) return
    await toast.promise(roomsApi.uploadPhoto(uploadingFor, file), {
      loading: 'Subiendo foto...',
      success: 'Foto subida',
      error: 'Error al subir la foto',
    })
    setUploadingFor(null)
    e.target.value = ''
    load()
  }

  const triggerUpload = (roomId: string) => {
    setUploadingFor(roomId)
    fileInputRef.current?.click()
  }

  const inputCls = (hasError: boolean) =>
    `w-full px-3 py-2.5 bg-white dark:bg-zinc-800 border rounded-lg text-sm text-zinc-900 dark:text-zinc-100 placeholder:text-zinc-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent ${
      hasError ? 'border-red-400' : 'border-zinc-300 dark:border-zinc-600'
    }`

  if (loading) {
    return (
      <div className="space-y-4">
        <div className="h-8 w-48 rounded-lg bg-zinc-200 dark:bg-zinc-800 animate-pulse" />
        <div className="h-40 rounded-xl bg-zinc-200 dark:bg-zinc-800 animate-pulse" />
      </div>
    )
  }

  if (!property) return null

  return (
    <div className="max-w-3xl">
      <input
        ref={fileInputRef}
        type="file"
        accept="image/jpeg,image/png,image/webp"
        className="hidden"
        onChange={handleUploadPhoto}
      />

      {/* Header */}
      <div className="flex items-center gap-3 mb-8">
        <Link href="/dashboard" className="text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors">
          <ArrowLeft size={20} />
        </Link>
        <div className="flex-1">
          <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 line-clamp-1">{property.name}</h1>
          <p className="text-sm text-zinc-500 dark:text-zinc-400 mt-0.5">
            {property.city}, {property.province}
          </p>
        </div>
        <Link
          href={`/propiedades/${property.id}`}
          target="_blank"
          className="flex items-center gap-1.5 text-sm text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors border border-zinc-200 dark:border-zinc-700 px-3 py-2 rounded-lg"
        >
          <Eye size={14} />
          Ver pública
        </Link>
      </div>

      {/* Rooms */}
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-zinc-900 dark:text-zinc-50">
          Habitaciones ({property.rooms.length})
        </h2>
        <button
          onClick={() => setShowAddRoom(v => !v)}
          className="flex items-center gap-1.5 text-sm font-medium text-red-600 dark:text-red-400 hover:text-red-700 transition-colors"
        >
          <Plus size={15} weight="bold" />
          {showAddRoom ? 'Cancelar' : 'Agregar habitación'}
        </button>
      </div>

      {/* Add room form */}
      {showAddRoom && (
        <form
          onSubmit={handleSubmit(onAddRoom)}
          className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 p-5 mb-5 space-y-4"
        >
          <h3 className="font-semibold text-zinc-900 dark:text-zinc-50 text-sm">Nueva habitación</h3>

          <div>
            <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">Título</label>
            <input {...register('title')} placeholder="Ej: Habitación doble con vista al lago" className={inputCls(!!errors.title)} />
            {errors.title && <p className="text-xs text-red-500 mt-1">{errors.title.message}</p>}
          </div>

          <div>
            <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">Descripción</label>
            <textarea {...register('description')} rows={3} placeholder="Describí la habitación..." className={`${inputCls(!!errors.description)} resize-none`} />
            {errors.description && <p className="text-xs text-red-500 mt-1">{errors.description.message}</p>}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">Capacidad (huéspedes)</label>
              <input type="number" {...register('capacity')} min={1} max={20} placeholder="2" className={inputCls(!!errors.capacity)} />
              {errors.capacity && <p className="text-xs text-red-500 mt-1">{errors.capacity.message}</p>}
            </div>
            <div>
              <label className="block text-xs font-medium text-zinc-500 dark:text-zinc-400 mb-1.5">Precio por noche ($)</label>
              <input type="number" {...register('pricePerNight')} min={1} placeholder="5000" className={inputCls(!!errors.pricePerNight)} />
              {errors.pricePerNight && <p className="text-xs text-red-500 mt-1">{errors.pricePerNight.message}</p>}
            </div>
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="px-5 py-2.5 bg-red-600 text-white text-sm font-semibold rounded-lg hover:bg-red-700 disabled:opacity-50 active:scale-[0.98] transition-all"
          >
            {isSubmitting ? 'Guardando...' : 'Guardar habitación'}
          </button>
        </form>
      )}

      {/* Room list */}
      {property.rooms.length === 0 ? (
        <div className="text-center py-12 bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800">
          <p className="text-zinc-400 text-sm">Todavía no hay habitaciones. Agregá la primera arriba.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {property.rooms.map(room => (
            <div
              key={room.id}
              className="bg-white dark:bg-zinc-900 rounded-xl border border-zinc-200 dark:border-zinc-800 p-4"
            >
              <div className="flex items-start gap-4">
                {/* Photos preview */}
                <div className="flex gap-1.5 shrink-0">
                  {room.coverPhotoUrl ? (
                    <div className="relative w-20 h-16 rounded-lg overflow-hidden bg-zinc-100 dark:bg-zinc-800">
                      <Image src={room.coverPhotoUrl} alt={room.title} fill className="object-cover" sizes="80px" />
                    </div>
                  ) : (
                    <div className="w-20 h-16 rounded-lg bg-zinc-100 dark:bg-zinc-800 flex items-center justify-center text-zinc-300">
                      <UploadSimple size={20} />
                    </div>
                  )}
                </div>

                {/* Info */}
                <div className="flex-1 min-w-0">
                  <p className="font-semibold text-zinc-900 dark:text-zinc-50 line-clamp-1">{room.title}</p>
                  <p className="text-xs text-zinc-500 dark:text-zinc-400 mt-0.5">
                    {room.capacity} huéspedes · ${room.pricePerNight.toLocaleString('es-AR')}/noche
                  </p>
                </div>

                {/* Actions */}
                <div className="flex items-center gap-2 shrink-0">
                  <button
                    onClick={() => triggerUpload(room.id)}
                    className="flex items-center gap-1.5 text-xs px-3 py-1.5 border border-zinc-200 dark:border-zinc-700 rounded-lg hover:bg-zinc-50 dark:hover:bg-zinc-800 text-zinc-600 dark:text-zinc-400 transition-colors"
                  >
                    <UploadSimple size={13} weight="bold" />
                    Foto
                  </button>
                  <button
                    onClick={() => handleDeleteRoom(room.id, room.title)}
                    className="p-1.5 text-zinc-400 hover:text-red-600 dark:hover:text-red-400 border border-zinc-200 dark:border-zinc-700 rounded-lg hover:border-red-200 dark:hover:border-red-800 transition-colors"
                  >
                    <Trash size={14} />
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
