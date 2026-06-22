# Estrategia de Desarrollo — BRB Habitaciones

## Filosofía de Desarrollo
- MVP primero: funcionalidad core antes que features avanzadas
- Deploy continuo desde fase 2: el staging en EasyPanel se actualiza con cada merge a `main`
- Mobile-first en el frontend: la mayoría de usuarios argentinos acceden desde celular
- Performance: optimizar para conexiones lentas (3G rural en provincias del interior)

## Fases de Lanzamiento

### v0.1 — MVP interno (Fase 1–3 del plan)
- Auth completo, publicación de habitaciones, búsqueda básica, reserva manual
- Solo para testing interno y primeros dueños piloto

### v0.2 — Beta pública (Fase 4–5)
- Calendario de disponibilidad, pagos integrados, panel de admin
- 10–50 dueños beta en 2–3 provincias

### v1.0 — Lanzamiento (Fase 6–7)
- Todas las features del plan general
- SEO optimizado, landing page, marketing básico

## Decisiones de Arquitectura Tomadas

### ¿Por qué Clean Architecture en .NET?
- Facilita testing unitario de la lógica de negocio sin tocar DB
- Permite cambiar ORM o base de datos con impacto mínimo
- Separa claramente las responsabilidades entre capas

### ¿Por qué Next.js App Router?
- Server-side rendering para SEO (las fichas de habitaciones deben aparecer en Google)
- React Server Components reducen el bundle de JS en el cliente
- Soporte nativo para rutas dinámicas y layouts

### ¿Por qué PostgreSQL sobre SQL Server?
- Gratuito y open source → sin licencia en el VPS
- Imagen Docker oficial liviana (postgres:16-alpine)
- Mejor soporte para búsquedas geoespaciales (PostGIS) si en el futuro queremos "buscar por radio")
- EasyPanel tiene template oficial para PostgreSQL

## Estrategia de Imágenes/Fotos
Ver decisión pendiente en `datos-actuales.md`.

## Integración de Pagos (futuro, no en v0.1)
- MercadoPago (dominante en Argentina, API madura)
- Opción: pagos en plataforma vs. acuerdo directo entre dueño y cliente
