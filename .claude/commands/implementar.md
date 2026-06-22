# Comando: Implementar

## Propósito
Implementar una tarea específica del plan de desarrollo, con confirmación previa y contexto completo.

## Uso
```
/implementar [descripción de lo que se quiere implementar]
```

## Instrucciones para el Agente

Cuando el usuario ejecute este comando:

1. **Leer contexto** (en este orden):
   - `.claude/planes/plan-general.md` → ubicar la tarea en la fase correcta
   - `CLAUDE.md` → respetar convenciones de código y arquitectura
   - Archivos relevantes de la capa a implementar

2. **Antes de escribir código**, mostrar al usuario:
   - Qué archivos se van a crear o modificar
   - Qué dependencias/paquetes se van a agregar (si aplica)
   - Impacto en otras partes del sistema
   - Preguntar: "¿Querés que proceda?"

3. **Al implementar**:
   - Respetar Clean Architecture (no saltarse capas)
   - Crear los tests correspondientes junto con el código
   - Agregar migraciones EF si se agregan entidades o se modifica el schema
   - Actualizar el `.env.example` si se agregan variables de entorno

4. **Al terminar**:
   - Listar los archivos creados/modificados
   - Indicar cómo testear el cambio manualmente
   - Marcar la tarea como completada en el plan correspondiente

## Reglas de Implementación

- NUNCA saltear Tests si la tarea los requiere
- NUNCA hardcodear secrets o credenciales
- NUNCA modificar migraciones ya aplicadas; crear nuevas
- Si hay duda arquitectural, preguntar antes de implementar
