# Plan General de Desarrollo — BRB Habitaciones

**Versión**: 1.0  
**Fecha**: 2026-06-17  
**Estado**: En planificación

---

## Resumen de Fases

| # | Fase | Contenido principal | Estado |
|---|------|--------------------|----|
| 1 | Setup e Infraestructura | Repos, DB, auth skeleton, Dockerfiles base | Pendiente |
| 2 | Backend Core | Entidades, migraciones, CRUD de propiedades/habitaciones | Pendiente |
| 3 | Frontend Core | Auth UI, búsqueda, listado, ficha de habitación | Pendiente |
| 4 | Flujo de Reservas | Calendario, creación/cancelación de reservas | Pendiente |
| 5 | Panel de Administración | Dashboard admin, gestión de usuarios, moderación | Pendiente |
| 6 | Dockerización y Deploy en EasyPanel | Dockerfiles finales, config de servicios, SSL | Pendiente |
| 7 | Calidad y Pulido | Tests, SEO, performance, accesibilidad | Pendiente |

---

## Fase 1: Setup e Infraestructura

### Objetivos
- Tener el proyecto compilando y corriendo localmente con Docker
- Auth básica funcionando end-to-end (registro + login + token JWT)
- Base de datos con schema inicial migrado

### Tareas Backend
- [ ] Crear solución .NET con 4 proyectos: `Api`, `Application`, `Domain`, `Infrastructure`
- [ ] Configurar EF Core con Npgsql (PostgreSQL)
- [ ] Crear entidades base: `User`, `RefreshToken`
- [ ] Configurar autenticación JWT (emisión + validación)
- [ ] Endpoints: `POST /api/v1/auth/register`, `POST /api/v1/auth/login`, `POST /api/v1/auth/refresh`
- [ ] Middleware de manejo de errores global (`ProblemDetails`)
- [ ] Health check endpoint: `GET /api/v1/health`
- [ ] Dockerfile del backend
- [ ] `docker-compose.yml` para desarrollo local (backend + postgres)

### Tareas Frontend
- [ ] Crear proyecto Next.js 14 con TypeScript + Tailwind CSS
- [ ] Configurar `next.config.js` con `output: 'standalone'`
- [ ] Layout base (header, footer, nav)
- [ ] Páginas de auth: `/login`, `/registro`
- [ ] Cliente HTTP (`/src/lib/api/`) con interceptor de token JWT
- [ ] Contexto de autenticación (Zustand o Context API)
- [ ] Dockerfile del frontend

### Entregables de Fase 1
- Registro e inicio de sesión funcionales end-to-end
- API corriendo en `localhost:5000`, frontend en `localhost:3000`
- `docker-compose up` levanta todo el stack

---

## Fase 2: Backend Core — Propiedades y Habitaciones

### Objetivos
- CRUD completo de propiedades y habitaciones
- Upload de fotos (integración con Cloudinary)
- Búsqueda por provincia/ciudad

### Tareas
- [ ] Entidades: `Property`, `Room`, `Photo`, `Amenity`, `RoomAmenity`
- [ ] Migraciones EF Core para todas las entidades
- [ ] Seed de datos: 24 provincias argentinas, amenities predefinidos
- [ ] Endpoints de propiedades:
  - `GET /api/v1/properties` (con filtros: provincia, ciudad, capacidad, precio)
  - `GET /api/v1/properties/{id}`
  - `POST /api/v1/properties` (rol: DuenoAlojamiento)
  - `PUT /api/v1/properties/{id}` (rol: DuenoAlojamiento, solo propias)
  - `DELETE /api/v1/properties/{id}` (soft delete)
- [ ] Endpoints de habitaciones:
  - `GET /api/v1/properties/{propertyId}/rooms`
  - `GET /api/v1/rooms/{id}`
  - `POST /api/v1/properties/{propertyId}/rooms`
  - `PUT /api/v1/rooms/{id}`
  - `DELETE /api/v1/rooms/{id}`
- [ ] Endpoint de upload de fotos: `POST /api/v1/rooms/{id}/photos` → Cloudinary
- [ ] Tests de integración para todos los endpoints

### Entregables de Fase 2
- API de propiedades y habitaciones completa y testeada
- Búsqueda por provincia/ciudad funcional
- Fotos subidas y almacenadas correctamente

---

## Fase 3: Frontend Core — Búsqueda y Ficha de Habitación

### Objetivos
- El usuario puede buscar habitaciones y ver los detalles
- Los dueños pueden publicar y gestionar sus propiedades

### Tareas
- [ ] Página de inicio con buscador (provincia, ciudad, fechas, huéspedes)
- [ ] Página de resultados `/buscar` con filtros laterales y cards de habitaciones
- [ ] Ficha de habitación `/habitaciones/[id]` con:
  - Galería de fotos
  - Descripción, amenities, ubicación (mapa static o texto)
  - Precio por noche
  - Selector de fechas (datepicker)
  - Botón "Reservar"
- [ ] Dashboard del dueño `/dashboard`:
  - Listado de mis propiedades
  - Formulario de nueva propiedad
  - Gestión de habitaciones y fotos
- [ ] Componentes UI reutilizables: Card, Badge, Modal, Spinner, Alert

### Entregables de Fase 3
- Flujo completo de búsqueda y visualización
- Dashboard del dueño funcional
- Responsive mobile-first

---

## Fase 4: Flujo de Reservas

### Objetivos
- Crear y cancelar reservas
- Calendario de disponibilidad por habitación
- Notificaciones básicas (email)

### Tareas Backend
- [ ] Entidades: `Reservation`, `Availability`
- [ ] Lógica de negocio:
  - Verificar disponibilidad antes de crear reserva
  - Bloquear fechas al confirmar reserva
  - Liberar fechas al cancelar
- [ ] Endpoints:
  - `GET /api/v1/rooms/{id}/availability?from=&to=` → fechas disponibles
  - `POST /api/v1/reservations` (rol: Cliente)
  - `GET /api/v1/reservations` (mis reservas — filtrado por rol)
  - `GET /api/v1/reservations/{id}`
  - `PUT /api/v1/reservations/{id}/cancel`
  - `PUT /api/v1/reservations/{id}/confirm` (rol: DuenoAlojamiento)
- [ ] Servicio de email (SendGrid o SMTP) para confirmaciones

### Tareas Frontend
- [ ] Calendario de disponibilidad en la ficha de habitación
- [ ] Flujo de checkout: selección de fechas → resumen → confirmación
- [ ] Sección "Mis reservas" en el dashboard del cliente
- [ ] Estados de reserva: Pendiente / Confirmada / Cancelada

### Entregables de Fase 4
- Reservas end-to-end funcionando
- Calendario actualizado en tiempo real
- Email de confirmación enviado

---

## Fase 5: Panel de Administración

### Objetivos
- Dashboard para el equipo interno de BRB
- Gestión de usuarios, propiedades y reservas desde un panel centralizado

### Tareas
- [ ] Ruta protegida `/admin` (solo rol Administrador)
- [ ] Dashboard con métricas: total usuarios, propiedades, reservas del mes, ingresos
- [ ] Gestión de usuarios: listar, cambiar rol, desactivar cuenta
- [ ] Gestión de propiedades: aprobar/rechazar publicaciones, moderar fotos
- [ ] Gestión de reservas: ver todas, intervenir en disputas
- [ ] Logs de actividad básicos

### Entregables de Fase 5
- Panel admin funcional y protegido
- Todas las acciones de moderación disponibles

---

## Fase 6: Dockerización y Deploy en EasyPanel

### Objetivos
- El proyecto completo corre en el VPS con EasyPanel
- HTTPS con SSL automático en cada servicio
- Variables de entorno configuradas desde el panel

### Dockerfile Backend (ASP.NET Core 8)

```dockerfile
# /backend/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BrbHabitaciones.Api/BrbHabitaciones.Api.csproj", "BrbHabitaciones.Api/"]
COPY ["BrbHabitaciones.Application/BrbHabitaciones.Application.csproj", "BrbHabitaciones.Application/"]
COPY ["BrbHabitaciones.Domain/BrbHabitaciones.Domain.csproj", "BrbHabitaciones.Domain/"]
COPY ["BrbHabitaciones.Infrastructure/BrbHabitaciones.Infrastructure.csproj", "BrbHabitaciones.Infrastructure/"]
RUN dotnet restore "BrbHabitaciones.Api/BrbHabitaciones.Api.csproj"
COPY . .
WORKDIR "/src/BrbHabitaciones.Api"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BrbHabitaciones.Api.dll"]
```

### Dockerfile Frontend (Next.js con standalone output)

```dockerfile
# /frontend/Dockerfile

FROM node:20-alpine AS deps
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci --only=production

FROM node:20-alpine AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
ENV NEXT_TELEMETRY_DISABLED=1
RUN npm run build

FROM node:20-alpine AS runner
WORKDIR /app
ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs
COPY --from=builder /app/public ./public
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static
USER nextjs
EXPOSE 3000
ENV PORT=3000
ENV HOSTNAME="0.0.0.0"
CMD ["node", "server.js"]
```

### Pasos de Configuración en EasyPanel

#### 1. Servicio: PostgreSQL
- Template oficial de EasyPanel: "PostgreSQL"
- Variables: `POSTGRES_DB=brb_habitaciones`, `POSTGRES_USER=brb`, `POSTGRES_PASSWORD=<secret>`
- Volumen persistente: `/var/lib/postgresql/data`
- Sin dominio público (solo red interna de EasyPanel)

#### 2. Servicio: Backend API
- Tipo: App (Dockerfile custom)
- Source: repositorio GitHub → carpeta `/backend`
- Dominio: `api.brbhabitaciones.com.ar` → puerto 8080
- SSL: Auto (Let's Encrypt vía EasyPanel)
- Variables de entorno (desde el panel):
  ```
  ASPNETCORE_ENVIRONMENT=Production
  CONNECTION_STRING=Host=postgres;Database=brb_habitaciones;Username=brb;Password=SECRET
  JWT_SECRET=...
  JWT_ISSUER=https://api.brbhabitaciones.com.ar
  JWT_AUDIENCE=https://brbhabitaciones.com.ar
  CLOUDINARY_CLOUD_NAME=...
  CLOUDINARY_API_KEY=...
  CLOUDINARY_API_SECRET=...
  CORS_ALLOWED_ORIGINS=https://brbhabitaciones.com.ar
  ```

#### 3. Servicio: Frontend
- Tipo: App (Dockerfile custom)
- Source: repositorio GitHub → carpeta `/frontend`
- Dominio: `brbhabitaciones.com.ar` → puerto 3000
- SSL: Auto (Let's Encrypt vía EasyPanel)
- Variables de entorno (desde el panel):
  ```
  NEXT_PUBLIC_API_URL=https://api.brbhabitaciones.com.ar
  NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME=...
  ```

#### 4. Migraciones en Producción
- Configurar EF Core para ejecutar migraciones automáticamente al arrancar (`app.MigrateDatabase()`)
- O ejecutar manualmente desde el terminal de EasyPanel: `dotnet ef database update`

### Entregables de Fase 6
- Los tres servicios corriendo en EasyPanel
- HTTPS funcionando en ambos dominios
- Deploy automático al hacer push a `main` (webhook de EasyPanel)

---

## Fase 7: Calidad y Pulido

### Tareas
- [ ] Tests unitarios del backend (xUnit): servicios de Application
- [ ] Tests de integración del backend (WebApplicationFactory + Testcontainers)
- [ ] SEO: metadata dinámica en fichas de habitación (Next.js `generateMetadata`)
- [ ] Open Graph tags para compartir en redes sociales
- [ ] Sitemap.xml dinámico
- [ ] Optimización de imágenes: `next/image` con Cloudinary loader
- [ ] Web Vitals: LCP < 2.5s, CLS < 0.1
- [ ] Accesibilidad WCAG 2.1 AA básica
- [ ] Manejo de errores en el frontend (error boundaries, páginas de error)
- [ ] Rate limiting en la API (ASP.NET Core rate limiter)
- [ ] Documentación de la API con Swagger/OpenAPI

### Entregables de Fase 7
- Cobertura de tests > 70% en backend
- Lighthouse score > 85 en todas las páginas principales
- API documentada con Swagger

---

## Modelo de Base de Datos

### Diagrama de Entidades (Mermaid)

```mermaid
erDiagram
    Users {
        uuid Id PK
        string Email UK
        string PasswordHash
        string FirstName
        string LastName
        string Phone
        string AvatarUrl
        enum Role "Cliente|DuenoAlojamiento|Administrador"
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
        datetime DeletedAt
    }

    RefreshTokens {
        uuid Id PK
        uuid UserId FK
        string Token UK
        datetime ExpiresAt
        datetime RevokedAt
        datetime CreatedAt
    }

    Properties {
        uuid Id PK
        uuid OwnerId FK
        string Name
        string Description
        string Province
        string City
        string Address
        decimal Latitude
        decimal Longitude
        enum PropertyType "Casa|Departamento|Hostel|Posada|Cabana|Otro"
        bool IsActive
        bool IsApproved
        datetime CreatedAt
        datetime UpdatedAt
        datetime DeletedAt
    }

    Rooms {
        uuid Id PK
        uuid PropertyId FK
        string Title
        string Description
        int Capacity
        decimal PricePerNight
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
        datetime DeletedAt
    }

    Photos {
        uuid Id PK
        uuid RoomId FK "nullable"
        uuid PropertyId FK "nullable"
        string Url
        string PublicId "Cloudinary ID"
        string AltText
        int DisplayOrder
        bool IsCover
        datetime CreatedAt
    }

    Amenities {
        uuid Id PK
        string Name
        string Icon "nombre del icono"
        string Category "Baño|Cocina|Conectividad|Seguridad|Entretenimiento|Otros"
    }

    RoomAmenities {
        uuid RoomId FK
        uuid AmenityId FK
    }

    Availability {
        uuid Id PK
        uuid RoomId FK
        date Date
        bool IsAvailable
        decimal PriceOverride "nullable, sobreescribe precio base"
    }

    Reservations {
        uuid Id PK
        uuid RoomId FK
        uuid ClientId FK
        date CheckInDate
        date CheckOutDate
        int GuestCount
        decimal TotalPrice
        enum Status "Pendiente|Confirmada|Cancelada|Completada"
        string CancellationReason "nullable"
        datetime CreatedAt
        datetime UpdatedAt
    }

    Reviews {
        uuid Id PK
        uuid ReservationId FK UK
        uuid AuthorId FK
        int Rating "1-5"
        string Comment
        datetime CreatedAt
    }

    Users ||--o{ Properties : "owns"
    Users ||--o{ Reservations : "makes"
    Users ||--o{ Reviews : "writes"
    Users ||--o{ RefreshTokens : "has"
    Properties ||--o{ Rooms : "contains"
    Properties ||--o{ Photos : "has"
    Rooms ||--o{ Photos : "has"
    Rooms ||--o{ RoomAmenities : "has"
    Amenities ||--o{ RoomAmenities : "used in"
    Rooms ||--o{ Availability : "has"
    Rooms ||--o{ Reservations : "booked in"
    Reservations ||--o| Reviews : "reviewed by"
```

### Índices Importantes
- `Users.Email` → UNIQUE INDEX
- `Properties(Province, City)` → INDEX compuesto para búsquedas
- `Availability(RoomId, Date)` → UNIQUE INDEX (no duplicar fechas por habitación)
- `Reservations(RoomId, CheckInDate, CheckOutDate)` → INDEX para verificar solapamientos
- `RefreshTokens.Token` → UNIQUE INDEX

### Seed de Datos Inicial
- 24 provincias argentinas en tabla de referencia (o enum en el dominio)
- ~30 amenities predefinidos (WiFi, Estacionamiento, Piscina, AC, TV, etc.)
- Usuario administrador por defecto (credenciales por variable de entorno)
