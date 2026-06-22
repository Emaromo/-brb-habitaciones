# BRB Habitaciones — Documento Maestro del Proyecto

## Resumen
Plataforma de reservas de habitaciones y alojamientos en Argentina. Permite a dueños publicar propiedades, a clientes buscar y reservar por provincia/ciudad, y a administradores gestionar todo el sistema.

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Backend | ASP.NET Core 8 Web API + Entity Framework Core 9 |
| Frontend | Next.js 14 (App Router, TypeScript) |
| Base de datos | PostgreSQL 16 |
| Autenticación | JWT (gestionado desde el backend .NET) |
| Imágenes | Cloudinary (free tier → paid según escala) |
| Deploy | VPS propio con EasyPanel (Docker) |
| CI/CD | GitHub Actions → EasyPanel webhooks |

## Estructura de Carpetas del Repo

```
/
├── CLAUDE.md                    ← este archivo
├── .claude/
│   ├── contexto/
│   │   ├── info-negocio.md
│   │   ├── estrategia.md
│   │   └── datos-actuales.md
│   ├── planes/
│   │   └── plan-general.md
│   ├── commands/
│   │   ├── crear-plan.md
│   │   ├── implementar.md
│   │   └── iniciar.md
│   ├── scripts/                 ← scripts de setup, migraciones, seed
│   └── skills/                  ← skills custom (vacío por ahora)
├── backend/                     ← ASP.NET Core Web API
│   ├── BrbHabitaciones.Api/
│   ├── BrbHabitaciones.Domain/
│   ├── BrbHabitaciones.Infrastructure/
│   ├── BrbHabitaciones.Application/
│   ├── Dockerfile
│   └── docker-compose.yml       ← solo para dev local
└── frontend/                    ← Next.js 14
    ├── src/
    │   ├── app/                 ← App Router
    │   ├── components/
    │   ├── lib/
    │   └── types/
    ├── Dockerfile
    └── next.config.js
```

## Convenciones de Código

### Backend (.NET)
- Clean Architecture: Api → Application → Domain ← Infrastructure
- Naming: PascalCase para clases/métodos, camelCase para variables locales
- DTOs en la capa Application, nunca exponer entidades de dominio directamente
- Endpoints RESTful con versionado: `/api/v1/...`
- Respuestas siempre con `ApiResponse<T>` wrapper
- Errores con `ProblemDetails` (RFC 7807)
- Migraciones EF Core en `Infrastructure/Migrations/`
- Todos los endpoints protegidos requieren `[Authorize]`
- Usar `record` para DTOs inmutables

### Frontend (Next.js)
- App Router (no Pages Router)
- Server Components por defecto, Client Components solo cuando necesario (`'use client'`)
- Naming: PascalCase para componentes, kebab-case para archivos de rutas
- Llamadas a API siempre a través de `/src/lib/api/`
- Variables de entorno públicas con prefijo `NEXT_PUBLIC_`
- Tailwind CSS para estilos, sin CSS modules salvo excepciones
- Zod para validación de formularios + React Hook Form
- Estado global solo con Zustand (si es necesario)

### Base de Datos
- Plural para nombres de tablas: `Users`, `Rooms`, `Reservations`
- Snake_case en columnas a nivel DB, PascalCase en entidades C#
- Soft delete: columna `DeletedAt` nullable (no borrar registros)
- Todas las tablas con `CreatedAt`, `UpdatedAt` auditables

## Roles del Sistema

| Rol | Descripción |
|-----|-------------|
| `Cliente` | Busca y reserva habitaciones |
| `DuenoAlojamiento` | Publica y gestiona sus propiedades/habitaciones |
| `Administrador` | Acceso completo al panel de administración |

## Reglas para el Agente (Claude)

1. **Nunca generar código de producción sin confirmación explícita del usuario**
2. Siempre consultar el plan en `.claude/planes/plan-general.md` antes de implementar
3. Trabajar fase por fase, no adelantarse a fases no confirmadas
4. Antes de cada fase nueva: mostrar qué se va a hacer y esperar confirmación
5. Los endpoints del backend deben tener tests de integración antes de avanzar a la siguiente fase
6. Al implementar migraciones EF Core, siempre revisar el SQL generado antes de aplicar
7. Las variables de entorno sensibles NUNCA van en el código; usar `.env.example` como referencia
8. Cada PR/commit debe referenciar la fase del plan que implementa
9. No instalar dependencias npm sin justificación explícita
10. Respetar Clean Architecture: no importar Infrastructure desde Domain ni Api desde Domain directamente
