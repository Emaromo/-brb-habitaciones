import type { Metadata } from 'next'
import { Outfit } from 'next/font/google'
import './globals.css'
import { Toaster } from 'react-hot-toast'

const outfit = Outfit({ subsets: ['latin'], display: 'swap' })

export const metadata: Metadata = {
  title: 'BRB Habitaciones — Alojamientos en Argentina',
  description: 'Encontrá y reservá habitaciones en toda Argentina.',
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
