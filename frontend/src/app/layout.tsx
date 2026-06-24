import type { Metadata } from 'next'
import { Outfit } from 'next/font/google'
import './globals.css'
import { Toaster } from 'react-hot-toast'

const outfit = Outfit({ subsets: ['latin'], display: 'swap' })

export const metadata: Metadata = {
  title: {
    default: 'BRB Habitaciones — Alojamientos en Argentina',
    template: '%s — BRB Habitaciones',
  },
  description: 'Encontrá y reservá habitaciones y alojamientos en toda Argentina. Casas, cabañas, hostels, posadas y más.',
  keywords: ['alojamiento argentina', 'habitaciones', 'reservas', 'cabañas', 'hostels', 'turismo argentina'],
  openGraph: {
    type: 'website',
    locale: 'es_AR',
    siteName: 'BRB Habitaciones',
    title: 'BRB Habitaciones — Alojamientos en Argentina',
    description: 'Encontrá y reservá habitaciones y alojamientos en toda Argentina.',
  },
  twitter: {
    card: 'summary_large_image',
  },
  robots: {
    index: true,
    follow: true,
  },
}

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="es">
      <body className={outfit.className}>
        {children}
        <Toaster position="top-right" />
      </body>
    </html>
  )
}
