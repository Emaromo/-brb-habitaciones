# Datos Actuales y Decisiones Pendientes

## Estado del Proyecto
- Fecha de inicio: 2026-06-17
- Fase actual: Planificación / Setup inicial
- Entorno de deploy: VPS propio con EasyPanel

## Decisión Pendiente 1: Base de Datos

**Recomendación: PostgreSQL 16**

| Criterio | PostgreSQL | SQL Server |
|---------|-----------|------------|
| Licencia | Gratuita | Gratis solo en Express (límite 10 GB) |
| Imagen Docker | `postgres:16-alpine` (~80 MB) | `mcr.microsoft.com/mssql/server` (~1.5 GB) |
| Soporte en EasyPanel | Template oficial disponible | Requiere config manual |
| Extensión geoespacial | PostGIS (potente y gratuita) | SQL Spatial (disponible pero complejo) |
| Performance en VPS pequeño | Excelente con poca RAM | Requiere mínimo 2 GB RAM solo para SQL Server |
| EF Core soporte | `Npgsql.EntityFrameworkCore.PostgreSQL` maduro | `Microsoft.EntityFrameworkCore.SqlServer` |

**Veredicto**: PostgreSQL es la opción óbvia para self-hosting en EasyPanel. SQL Server solo tendría sentido en Azure.

---

## Decisión Pendiente 2: Almacenamiento de Imágenes

### Opción A: Cloudinary (recomendada para MVP)
**¿Qué es?** CDN + transformación de imágenes en la nube (resize, crop, webp automático).

**Pros:**
- Free tier: 25 créditos/mes (~25.000 transformaciones + 25 GB ancho de banda + 10 GB almacenamiento)
- SDK oficial para .NET y JS
- Optimización automática (webp, resize, lazy load)
- No requiere configurar nada en el VPS

**Contras:**
- Dependencia de servicio externo (si Cloudinary cae, no se ven fotos)
- Al superar free tier: ~$89 USD/mes (plan Plus)

**Costo estimado:**
- 0–500 propiedades: $0/mes (free tier alcanza)
- 500–5.000 propiedades: $25–89 USD/mes (Plus)

---

### Opción B: AWS S3 + CloudFront
**¿Qué es?** Almacenamiento en S3, distribución por CDN CloudFront.

**Pros:**
- Escalabilidad infinita
- Pay-per-use real (sin límites de tier)
- Control total

**Contras:**
- Setup más complejo (IAM, buckets, políticas, CloudFront distribution)
- Sin transformación automática de imágenes (hay que agregar Lambda@Edge o imgproxy)
- Requiere manejo de credenciales AWS

**Costo estimado:**
- S3: ~$0.023/GB almacenado + $0.09/GB transferido
- Para 1.000 fotos (~500 MB): < $1 USD/mes
- Para 50.000 fotos (~25 GB): ~$3 USD/mes en storage + tráfico variable

---

### Opción C: Volumen persistente en el VPS (EasyPanel)
**¿Qué es?** Las fotos se guardan en el filesystem del VPS, servidas por el backend o Nginx.

**Pros:**
- Costo $0 (ya pagás el VPS)
- Sin dependencias externas
- Control total sobre los archivos

**Contras:**
- No escala: el VPS tiene espacio de disco limitado
- Sin CDN: todas las requests van al VPS (puede saturar el ancho de banda)
- Sin optimización automática de imágenes
- **Riesgo crítico**: si se borra el volumen o se pierde el VPS, se pierden todas las fotos
- Backups manuales necesarios
- Servir imágenes consume recursos del mismo servidor que corre la API

**Costo estimado:**
- $0 adicional, pero limita el espacio total disponible en el VPS

---

### Recomendación del Agente
**Para MVP (0–1.000 propiedades)**: Opción A (Cloudinary free tier), costo $0.  
**Para crecimiento (1.000+ propiedades)**: Migrar a Opción B (S3 + CloudFront), ~$3–10 USD/mes.  
**Opción C**: No recomendada para producción. Aceptable solo para desarrollo local.

**→ Decisión del dueño del proyecto: PENDIENTE**

---

## Variables de Entorno Necesarias (referencia)

```env
# Backend
ASPNETCORE_ENVIRONMENT=Production
CONNECTION_STRING=Host=postgres;Database=brb_habitaciones;Username=brb;Password=SECRET
JWT_SECRET=CAMBIAR_POR_SECRET_LARGO_ALEATORIO
JWT_ISSUER=https://api.brbhabitaciones.com.ar
JWT_AUDIENCE=https://brbhabitaciones.com.ar
CLOUDINARY_CLOUD_NAME=
CLOUDINARY_API_KEY=
CLOUDINARY_API_SECRET=
CORS_ALLOWED_ORIGINS=https://brbhabitaciones.com.ar

# Frontend
NEXT_PUBLIC_API_URL=https://api.brbhabitaciones.com.ar
NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME=
```
