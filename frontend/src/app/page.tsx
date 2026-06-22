import Image from 'next/image'
import Link from 'next/link'
import { MapPin, CalendarBlank, ShieldCheck } from '@phosphor-icons/react/dist/ssr'
import SearchBar from '@/components/SearchBar'
import Navbar from '@/components/Navbar'

const FEATURED_CITIES = [
  {
    name: 'Buenos Aires',
    province: 'Buenos Aires',
    img: 'https://picsum.photos/seed/buenos-aires-city-arg/600/400',
    count: 47,
  },
  {
    name: 'Bariloche',
    province: 'Río Negro',
    img: 'https://picsum.photos/seed/bariloche-patagonia-arg/600/400',
    count: 31,
  },
  {
    name: 'Mendoza',
    province: 'Mendoza',
    img: 'https://picsum.photos/seed/mendoza-vino-arg/600/400',
    count: 28,
  },
  {
    name: 'Salta',
    province: 'Salta',
    img: 'https://picsum.photos/seed/salta-noroeste-arg/600/400',
    count: 19,
  },
]

const STEPS = [
  {
    title: 'Buscá tu destino',
    body: 'Ingresá la provincia o ciudad. Filtrá por precio, capacidad y fecha de entrada.',
    Icon: MapPin,
  },
  {
    title: 'Revisá y reservá',
    body: 'Mirá fotos reales, leé reseñas verificadas y confirmá en minutos desde el celular.',
    Icon: CalendarBlank,
  },
  {
    title: 'Llegá sin sorpresas',
    body: 'Coordiná con el dueño directo. Sin comisiones ocultas ni cargos en dólares.',
    Icon: ShieldCheck,
  },
]

const TESTIMONIALS = [
  {
    quote:
      '“Encontré una habitación en Mendoza en 5 minutos. El dueño respondió al toque y el precio era justo.”',
    name: 'Valentina Ríos',
    location: 'Córdoba',
    img: 'https://picsum.photos/seed/valentina-rios-arg/80/80',
  },
  {
    quote:
      '“Publiqué mis dos habitaciones y en la primera semana ya tuve reservas. La plataforma es muy fácil de usar.”',
    name: 'Roberto Alderete',
    location: 'Tucumán',
    img: 'https://picsum.photos/seed/roberto-alderete-tucuman/80/80',
  },
]

export default function HomePage() {
  return (
    <div className="min-h-[100dvh] bg-zinc-50 dark:bg-zinc-950 text-zinc-900 dark:text-zinc-100">

      <Navbar />

      {/* ── HERO ── asymmetric split: copy left, photo right */}
      <section className="max-w-7xl mx-auto px-4 sm:px-6 pt-12 pb-16 lg:pt-16 lg:pb-24">
        <div className="grid grid-cols-1 lg:grid-cols-[1fr_400px] gap-10 items-center">

          {/* Left: headline + search + city pills */}
          <div className="max-w-xl">
            <h1 className="text-4xl sm:text-5xl lg:text-[3.5rem] font-bold tracking-tight text-zinc-900 dark:text-zinc-50 leading-[1.1]">
              Alojamientos en<br />toda Argentina
            </h1>
            <p className="mt-4 text-lg text-zinc-500 dark:text-zinc-400 leading-relaxed">
              Buscá por provincia, reservá en minutos. Sin comisiones en dólares.
            </p>

            <div className="mt-8">
              <SearchBar />
            </div>

            <div className="mt-4 flex flex-wrap gap-2">
              {['Buenos Aires', 'Córdoba', 'Mendoza', 'Bariloche', 'Salta'].map(city => (
                <Link
                  key={city}
                  href={`/buscar?q=${encodeURIComponent(city)}`}
                  className="px-3 py-1.5 bg-white dark:bg-zinc-800 border border-zinc-200 dark:border-zinc-700 rounded-full text-sm text-zinc-600 dark:text-zinc-300 hover:border-red-400 hover:text-red-600 dark:hover:border-red-500 dark:hover:text-red-400 transition-colors"
                >
                  {city}
                </Link>
              ))}
            </div>
          </div>

          {/* Right: hero photo — hidden on mobile */}
          <div className="hidden lg:block relative h-[420px] rounded-2xl overflow-hidden shadow-lg">
            <Image
              src="https://picsum.photos/seed/argentina-habitacion-living/800/840"
              alt="Habitación acogedora en Argentina"
              fill
              className="object-cover"
              priority
              sizes="400px"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-zinc-900/30 to-transparent" />
          </div>
        </div>
      </section>

      {/* ── DESTINOS POPULARES ── image card grid */}
      <section className="py-16 bg-white dark:bg-zinc-900">
        <div className="max-w-7xl mx-auto px-4 sm:px-6">
          <h2 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-8">
            Destinos populares
          </h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            {FEATURED_CITIES.map(city => (
              <Link
                key={city.name}
                href={`/buscar?q=${encodeURIComponent(city.name)}`}
                className="group relative h-52 rounded-xl overflow-hidden block"
              >
                <Image
                  src={city.img}
                  alt={city.name}
                  fill
                  className="object-cover group-hover:scale-105 transition-transform duration-500"
                  sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 25vw"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-zinc-900/70 via-zinc-900/20 to-transparent" />
                <div className="absolute bottom-0 left-0 p-4">
                  <p className="text-white font-semibold text-base leading-tight">{city.name}</p>
                  <p className="text-zinc-300 text-sm mt-0.5">{city.count} alojamientos</p>
                </div>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* ── CÓMO FUNCIONA ── numbered icon+text list (not 3 equal cards) */}
      <section className="py-16 bg-zinc-50 dark:bg-zinc-950">
        <div className="max-w-2xl mx-auto px-4 sm:px-6">
          <h2 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-10">
            Reservar es simple
          </h2>
          <div className="space-y-8">
            {STEPS.map(({ title, body, Icon }, i) => (
              <div key={title} className="grid grid-cols-[3rem_1fr] gap-5 items-start">
                <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-red-50 dark:bg-red-950 text-red-600 dark:text-red-400 shrink-0">
                  <Icon size={22} weight="duotone" />
                </div>
                <div>
                  <p className="text-xs font-semibold text-zinc-400 dark:text-zinc-500 tracking-widest uppercase mb-1">
                    {String(i + 1).padStart(2, '0')}
                  </p>
                  <h3 className="text-base font-semibold text-zinc-900 dark:text-zinc-50 mb-1">
                    {title}
                  </h3>
                  <p className="text-sm text-zinc-500 dark:text-zinc-400 leading-relaxed">{body}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ── TESTIMONIALES ── 2-col quote cards */}
      <section className="py-16 bg-white dark:bg-zinc-900">
        <div className="max-w-7xl mx-auto px-4 sm:px-6">
          <h2 className="text-2xl font-bold text-zinc-900 dark:text-zinc-50 mb-8">
            Lo que dicen quienes ya usaron BRB
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-5">
            {TESTIMONIALS.map(t => (
              <div
                key={t.name}
                className="bg-zinc-50 dark:bg-zinc-800 rounded-xl p-6 border border-zinc-200 dark:border-zinc-700"
              >
                <p className="text-zinc-700 dark:text-zinc-300 leading-relaxed text-sm mb-5">
                  {t.quote}
                </p>
                <div className="flex items-center gap-3">
                  <div className="relative w-9 h-9 rounded-full overflow-hidden shrink-0">
                    <Image
                      src={t.img}
                      alt={t.name}
                      fill
                      className="object-cover"
                      sizes="36px"
                    />
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-zinc-900 dark:text-zinc-50 leading-tight">
                      {t.name}
                    </p>
                    <p className="text-xs text-zinc-500 dark:text-zinc-400">{t.location}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ── OWNER CTA ── full-width dark section */}
      <section className="py-20 bg-zinc-900 dark:bg-zinc-800">
        <div className="max-w-2xl mx-auto px-4 sm:px-6 text-center">
          <h2 className="text-3xl font-bold text-white leading-tight mb-3">
            ¿Tenés una propiedad en Argentina?
          </h2>
          <p className="text-zinc-400 text-lg mb-8">
            Publicá tus habitaciones gratis y empezá a recibir reservas esta semana.
          </p>
          <Link
            href="/dashboard"
            className="inline-block bg-red-600 text-white px-8 py-3.5 rounded-lg font-semibold hover:bg-red-700 active:scale-[0.98] transition-all"
          >
            Publicá gratis
          </Link>
        </div>
      </section>

      {/* ── FOOTER ── */}
      <footer className="py-8 bg-zinc-950 border-t border-zinc-800">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 flex flex-col sm:flex-row items-center justify-between gap-4 text-sm text-zinc-500">
          <span className="font-semibold text-zinc-300">BRB Habitaciones</span>
          <div className="flex gap-6">
            <Link href="/login" className="hover:text-zinc-300 transition-colors">
              Iniciar sesión
            </Link>
            <Link href="/registro" className="hover:text-zinc-300 transition-colors">
              Registrarse
            </Link>
            <Link href="/dashboard" className="hover:text-zinc-300 transition-colors">
              Publicar propiedad
            </Link>
          </div>
          <span>© 2026 BRB Habitaciones</span>
        </div>
      </footer>

    </div>
  )
}
