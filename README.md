# ProjectTaskManager

API REST para la gestión de proyectos y tareas desarrollada en **.NET 10** siguiendo los principios de **Clean Architecture**.

---

## 🏛️ Arquitectura del Sistema

La solución está estructurada en cuatro capas independientes con inversión de dependencias:

```
src/
├── ProjectTaskManager.Domain/           # Capa de Dominio (Núcleo)
├── ProjectTaskManager.Application/      # Capa de Aplicación (Casos de uso y reglas de negocio)
├── ProjectTaskManager.Infrastructure/   # Capa de Infraestructura (EF Core y PostgreSQL)
└── ProjectTaskManager.API/              # Capa de Presentación (Controladores y Swagger)
```

### 1. Dominio (`Domain`)
- **Entidades**:
  - `Proyecto`: Nombre, descripción, fechas de inicio/fin y colección de tareas asociadas.
  - `Tarea`: Título, descripción, estado, prioridad, fecha límite y referencia a su proyecto.
- **Identificadores**: Uso de **UUIDv7** (`Guid.CreateVersion7()`), ordenables cronológicamente y optimizados para índices de base de datos.
- **Enums**: `EstadoTarea` (Pendiente, EnProgreso, Completada) y `Prioridad` (Baja, Media, Alta).

### 2. Aplicación (`Application`)
- **DTOs**: Contratos de entrada y salida desacoplados de las entidades de persistencia.
- **Excepciones de dominio**: `NotFoundException` y `BusinessRuleException`.
- **Servicios e Interfaces**:
  - `ProyectoService` / `IProyectoService`: CRUD de proyectos, cálculo de avance porcentual y validaciones.
  - `TareaService` / `ITareaService`: CRUD de tareas, cambio de estado individual y filtros por estado/prioridad.
- **Reglas de negocio implementadas**:
  - **Cálculo de avance**: Porcentaje dinámico de tareas completadas respecto al total.
  - **No borrar con pendientes**: Impide eliminar proyectos que contengan tareas no completadas.
  - **Consulta de tareas vencidas**: Detección de tareas cuya fecha límite expiró sin haberse completado.
  - **Validación de proyecto**: Verifica la existencia del proyecto antes de asociar una tarea.

### 3. Infraestructura (`Infrastructure`)
- **Persistencia**: Entity Framework Core con proveedor PostgreSQL (`Npgsql`).
- **`AppDbContext`**: Mapeo relacional y configuración vía Fluent API (`ProyectoConfiguration`, `TareaConfiguration`).
- **Políticas de integridad**: Conversión de enums a strings y restricción de borrado en cascada (`DeleteBehavior.Restrict`).
- **Repositorios**: Implementaciones de `IProyectoRepository` y `ITareaRepository` con consultas optimizadas (`AsNoTracking`).

### 4. API (`API`)
- **Controladores REST**:
  - `/api/proyectos`: CRUD de proyectos, consulta de tareas por proyecto y cálculo de avance.
  - `/api/tareas`: CRUD de tareas, listado de vencidas, filtros por query y cambio de estado vía `PATCH`.
- **Middleware global**: `ExceptionHandlingMiddleware` captura excepciones y responde en formato estándar `ProblemDetails` (`application/problem+json`):
  - `404 Not Found` para recursos inexistentes.
  - `409 Conflict` para violaciones de reglas de negocio.
  - `400 Bad Request` para solicitudes inválidas.
- **Documentación interactiva**: Swagger / OpenAPI habilitado con soporte para comentarios XML.

---

## 🚀 Puesta en Marcha

### Requisitos previos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Servidor PostgreSQL en ejecución.

### Configuración
Actualiza la cadena de conexión en `src/ProjectTaskManager.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=projecttaskmanager;Username=postgres;Password=tu_password"
}
```

### Ejecutar la aplicación
```bash
dotnet build
dotnet run --project src/ProjectTaskManager.API
```
Accede a Swagger UI en el navegador:
```
https://localhost:7042/swagger
```
