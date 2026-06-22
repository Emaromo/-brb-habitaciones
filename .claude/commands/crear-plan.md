# Comando: Crear Plan

## Propósito
Generar o actualizar el plan de desarrollo para una fase específica del proyecto.

## Uso
```
/crear-plan [fase-numero]
```

## Instrucciones para el Agente

Cuando el usuario ejecute este comando:

1. Leer el archivo `.claude/planes/plan-general.md` para entender el contexto
2. Leer `.claude/contexto/info-negocio.md` para alinear con objetivos de negocio
3. Preguntar al usuario: ¿Qué fase querés planificar con más detalle?
4. Generar un plan detallado de sprint (1–2 semanas) con:
   - Lista de tareas ordenadas por dependencia
   - Estimación de tiempo por tarea (en horas)
   - Criterios de aceptación para cada tarea
   - Riesgos y bloqueadores potenciales
5. Guardar el plan en `.claude/planes/fase-[N]-sprint-[M].md`
6. Actualizar el estado de la fase en `plan-general.md`

## Formato del Plan de Sprint

```markdown
# Fase [N] — Sprint [M]: [Título]
**Fechas**: [inicio] → [fin]
**Objetivo**: [una oración]

## Tareas

### Backend
- [ ] [tarea] — [Xh] — AC: [criterio]

### Frontend
- [ ] [tarea] — [Xh] — AC: [criterio]

## Dependencias externas
## Riesgos
## Definición de Done
```
