'use client'

import { useRouter } from 'next/navigation'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import Link from 'next/link'
import { ArrowLeft } from '@phosphor-icons/react'
import { propertiesApi } from '@/lib/api/properties'
import { ARGENTINA_PROVINCES, PROPERTY_TYPES } from '@/lib/constants'
import toast from 'react-hot-toast'

const schema = z.object({
  name: z.string().min(3, 'Mínimo 3 caracteres').max(200),
  description: z.string().min(20, 'Describí la propiedad con más detalle').max(2000),
  province: z.string().min(1, 'Elegí una provincia'),
  city: z.string().min(2, 'Ingresá la ciudad').max(100),
  address: z.string().min(5, 'Ingresá la dirección completa').max(300),
  propertyType: z.string().min(1, 'Elegí un tipo'),
})

type FormData = z.infer<typeof schema>

export default function NuevaPropiedadPage() {
  const router = useRouter()
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({ resolver: zodResolver(schema) })

  const onSubmit = async (data: FormData) => {
    try {
      const created = await propertiesApi.create(data)
      toast.success('Propiedad publicada')
      router.push(`/dashboard/propiedades/${created.id}`)
    } catch {
      toast.error('No se pudo crear la propiedad. Verificá tu rol de cuenta.')
    }
  }

  const inputCls = (hasError: boolean) =>
    `w-full px-3 py-2.5 bg-white dark:bg-zinc-800 border rounded-lg text-sm text-zinc-900 dark:text-zinc-100 placeholder:text-zinc-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent transition-colors ${
      hasError
        ? 'border-red-400 dark:border-red-600'
        : 'border-zinc-300 dark:border-zinc-600'
    }`

  return (
    <div className="max-w-2xl">
      <div className="flex items-center gap-3 mb-8">
        <Link
          href="/dashboard"
          className="text-zinc-500 hover:text-zinc-900 dark:hover:text-zinc-100 transition-colors"
        >
          <ArrowLeft size={20} />
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">Nueva propiedad</h1>
          <p className="text-sm text-zinc-500 dark:text-zinc-400 mt-0.5">
            Completá los datos básicos. Después podés agregar habitaciones y fotos.
          </p>
        </div>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <div>
          <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-300 mb-1.5">
            Nombre de la propiedad
          </label>
          <input
            {...register('name')}
            placeholder="Ej: Casa de playa en Mar del Plata"
            className={inputCls(!!errors.name)}
          />
          {errors.name && <p className="text-xs text-red-500 mt-1">{errors.name.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-300 mb-1.5">
            Descripción
          </label>
          <textarea
            {...register('description')}
            rows={4}
            placeholder="Describí el ambiente, la ubicación, qué tiene de especial tu propiedad..."
            className={`${inputCls(!!errors.description)} resize-none`}
          />
          {errors.description && <p className="text-xs text-red-500 mt-1">{errors.description.message}</p>}
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-300 mb-1.5">
              Tipo de propiedad
            </label>
            <select {...register('propertyType')} className={inputCls(!!errors.propertyType)}>
              <option value="">Seleccioná un tipo</option>
              {PROPERTY_TYPES.map(t => (
                <option key={t.value} value={t.value}>{t.label}</option>
              ))}
            </select>
            {errors.propertyType && <p className="text-xs text-red-500 mt-1">{errors.propertyType.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-300 mb-1.5">
              Provincia
            </label>
            <select {...register('province')} className={inputCls(!!errors.province)}>
              <option value="">Elegí la provincia</option>
              {ARGENTINA_PROVINCES.map(p => (
                <option key={p} value={p}>{p}</option>
              ))}
            </select>
            {errors.province && <p className="text-xs text-red-500 mt-1">{errors.province.message}</p>}
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-300 mb-1.5">
              Ciudad
            </label>
            <input
              {...register('city')}
              placeholder="Ej: Bariloche"
              className={inputCls(!!errors.city)}
            />
            {errors.city && <p className="text-xs text-red-500 mt-1">{errors.city.message}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-zinc-700 dark:text-zinc-300 mb-1.5">
              Dirección
            </label>
            <input
              {...register('address')}
              placeholder="Ej: Av. San Martín 1234"
              className={inputCls(!!errors.address)}
            />
            {errors.address && <p className="text-xs text-red-500 mt-1">{errors.address.message}</p>}
          </div>
        </div>

        <div className="pt-2 flex items-center gap-3">
          <button
            type="submit"
            disabled={isSubmitting}
            className="px-6 py-3 bg-red-600 text-white text-sm font-semibold rounded-lg hover:bg-red-700 disabled:opacity-50 active:scale-[0.98] transition-all"
          >
            {isSubmitting ? 'Publicando...' : 'Publicar propiedad'}
          </button>
          <Link
            href="/dashboard"
            className="text-sm text-zinc-500 hover:text-zinc-700 dark:hover:text-zinc-300 transition-colors"
          >
            Cancelar
          </Link>
        </div>
      </form>
    </div>
  )
}
