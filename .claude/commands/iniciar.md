# Comando: Iniciar

## Propósito
Arrancar el entorno de desarrollo local completo (backend + frontend + base de datos).

## Uso
```
/iniciar
```

## Instrucciones para el Agente

Cuando el usuario ejecute este comando:

1. Verificar que existan los archivos necesarios:
   - `backend/docker-compose.yml`
   - `backend/.env` (o variables configuradas)
   - `frontend/.env.local`

2. Si falta algún archivo, indicar qué falta y cómo crearlo (sin crearlo automáticamente)

3. Ejecutar el stack en orden:
   ```bash
   # Desde /backend
   docker-compose up -d postgres    # primero la DB
   dotnet run --project BrbHabitaciones.Api    # luego la API
   
   # Desde /frontend
   npm run dev
   ```

4. Verificar que los servicios respondan:
   - Backend: `GET http://localhost:5000/api/v1/health`
   - Frontend: `http://localhost:3000`

5. Si algo falla, mostrar los logs relevantes y sugerir solución

## URLs del Entorno Local

| Servicio | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| PostgreSQL | localhost:5432 (usuario: brb, db: brb_habitaciones) |
