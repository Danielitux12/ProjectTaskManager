# ProjectTaskManager 🚀

API REST para la gestión integral de proyectos y tareas desarrollada en **.NET 10** y **PostgreSQL**, construida bajo los principios de **Clean Architecture** (Arquitectura Limpia) y diseño guiado por el dominio.

---

## 🏛️ Arquitectura del Sistema

La solución se divide en cuatro capas desacopladas con inversión de dependencias:

```text
ProjectTaskManager/
├── src/
│   ├── ProjectTaskManager.Domain/          # Núcleo: Entidades, Enums y Reglas del Negocio
│   ├── ProjectTaskManager.Application/     # Casos de uso: DTOs, Servicios, Interfaces y Excepciones
│   ├── ProjectTaskManager.Infrastructure/  # Acceso a datos: EF Core, DbContext, Mapeos y Repositorios
│   └── ProjectTaskManager.API/             # Presentación: Controladores REST, Middlewares y Swagger
├── docker-compose.yml                      # Contenedor listo para PostgreSQL
└── ProjectTaskManager.slnx                 # Solución .NET
```

### 1. Dominio (`Domain`)
- **Entidades**:
  - `Proyecto`: Nombre, descripción, fechas de inicio y fin, y colección de tareas asociadas.
  - `Tarea`: Título, descripción, estado, prioridad, fecha límite y clave foránea al proyecto.
- **Identificadores Modernos**: Uso de **UUIDv7** (`Guid.CreateVersion7()`), que combina ordenación cronológica secuencial con unicidad global, optimizando el rendimiento de índices B-Tree en PostgreSQL.
- **Enums tipados**:
  - `EstadoTarea`: `Pendiente`, `EnProgreso`, `Completada`.
  - `Prioridad`: `Baja`, `Media`, `Alta`.

### 2. Aplicación (`Application`)
- **DTOs (Data Transfer Objects)**: Contratos inmutables (`records`) para entrada y salida (`ProyectoDto`, `CrearProyectoDto`, `TareaDto`, `CrearTareaDto`, `CambiarEstadoTareaDto`).
- **Servicios e Interfaces**:
  - `IProyectoService` / `ProyectoService`: Gestión de proyectos, cálculo de avance porcentual y validaciones de negocio.
  - `ITareaService` / `TareaService`: CRUD de tareas, cambio granular de estado y consultas por filtros/vencidas.
- **Excepciones de Negocio**:
  - `NotFoundException`: Disparada ante recursos inexistentes.
  - `BusinessRuleException`: Disparada al violar una política o restricción del negocio.

### 3. Infraestructura (`Infrastructure`)
- **Persistencia**: Entity Framework Core con el proveedor oficial de PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL`).
- **Configuración Fluida (`IEntityTypeConfiguration`)**:
  - Conversión automática de enumeradores a `string` en base de datos.
  - Reglas de restricción de integridad referencial: `DeleteBehavior.Restrict` (evita borrados en cascada no controlados).
- **Patrón Repositorio**: `ProyectoRepository` y `TareaRepository` con optimizaciones de lectura (`AsNoTracking`).

### 4. API (`API`)
- **Controladores REST Versionados**: Todos los endpoints están bajo el prefijo `/api/v1/`.
- **Manejo Global de Excepciones**: Middleware centralizado (`ExceptionHandlingMiddleware`) que estandariza las respuestas de error en formato **RFC 7807 (ProblemDetails)**.
- **Swagger / OpenAPI**: Documentación interactiva habilitada con soporte para comentarios XML descriptivos.

---

## 📋 Reglas de Negocio Implementadas

1. **Impedir eliminación de proyectos con tareas pendientes:**
   - Un proyecto no puede eliminarse si contiene tareas en estado `Pendiente` o `EnProgreso`.
   - Si se intenta eliminar, la API responde con un código HTTP `409 Conflict` indicando que existen tareas sin completar.
2. **Cálculo dinámico de avance:**
   - Cálculo en tiempo real del porcentaje de progreso del proyecto: `(Tareas Completadas / Total de Tareas) * 100`, redondeado a 2 decimales.
   - Retorna `0%` si el proyecto aún no registra tareas.
3. **Consulta de tareas vencidas:**
   - Identifica tareas cuya `FechaLimite` es inferior a `DateTime.UtcNow` y cuyo estado no sea `Completada`.
4. **Validación relacional:**
   - Al crear una tarea, se valida previamente la existencia del proyecto asociado en base de datos; si no existe, devuelve `404 Not Found`.

---

## 📡 Catálogo de Endpoints (`/api/v1`)

### 📁 Proyectos (`/api/v1/proyectos`)

| Método | Endpoint | Descripción | Respuestas |
|---|---|---|---|
| `GET` | `/api/v1/proyectos` | Lista todos los proyectos | `200 OK` |
| `GET` | `/api/v1/proyectos/{id}` | Obtiene el detalle de un proyecto por su UUID | `200 OK`, `404 Not Found` |
| `POST` | `/api/v1/proyectos` | Crea un nuevo proyecto | `201 Created` |
| `PUT` | `/api/v1/proyectos/{id}` | Actualiza la información de un proyecto existente | `204 NoContent`, `404 Not Found` |
| `DELETE` | `/api/v1/proyectos/{id}` | Elimina un proyecto (bloqueado si tiene tareas pendientes) | `204 NoContent`, `404 Not Found`, `409 Conflict` |
| `GET` | `/api/v1/proyectos/{id}/tareas` | Lista todas las tareas pertenecientes a un proyecto | `200 OK` |
| `GET` | `/api/v1/proyectos/{id}/avance` | Devuelve el porcentaje numérico de avance del proyecto | `200 OK`, `404 Not Found` |

### 📝 Tareas (`/api/v1/tareas`)

| Método | Endpoint | Descripción | Parámetros / Body | Respuestas |
|---|---|---|---|---|
| `GET` | `/api/v1/tareas` | Lista tareas con filtros opcionales | Query: `?estado=...&prioridad=...` | `200 OK` |
| `GET` | `/api/v1/tareas/vencidas` | Lista las tareas vencidas que no están completadas | Ninguno | `200 OK` |
| `GET` | `/api/v1/tareas/{id}` | Obtiene una tarea por su UUID | Ninguno | `200 OK`, `404 Not Found` |
| `POST` | `/api/v1/tareas` | Crea una nueva tarea (estado inicial: `Pendiente`) | Body: `CrearTareaDto` | `201 Created`, `404 Not Found` |
| `PUT` | `/api/v1/tareas/{id}` | Modifica los datos de una tarea existente | Body: `CrearTareaDto` | `204 NoContent`, `404 Not Found` |
| `PATCH` | `/api/v1/tareas/{id}/estado` | Actualiza exclusivamente el estado de una tarea | Body: `{"nuevoEstado": "EnProgreso"}` | `204 NoContent`, `404 Not Found` |
| `DELETE` | `/api/v1/tareas/{id}` | Elimina una tarea | Ninguno | `204 NoContent`, `404 Not Found` |

---

## 🛡️ Manejo de Errores (ProblemDetails)

Los errores son interceptados por el `ExceptionHandlingMiddleware` retornando la cabecera `Content-Type: application/problem+json`:

```json
{
  "title": "Regla de negocio violada",
  "status": 409,
  "detail": "No se puede eliminar el proyecto: tiene tareas sin completar."
}
```

| Código HTTP | Causa | Título retornado |
|---|---|---|
| **400 Bad Request** | Argumentos o sintaxis inválida (`ArgumentException`) | `Solicitud inválida` |
| **404 Not Found** | El recurso no existe en el sistema (`NotFoundException`) | `Recurso no encontrado` |
| **409 Conflict** | Violación de restricciones de negocio (`BusinessRuleException`) | `Regla de negocio violada` |
| **500 Internal Server Error** | Excepciones no controladas | `Error interno del servidor` |

---

## 🚀 Puesta en Marcha

### Requisitos previos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker & Docker Compose](https://www.docker.com/) (o una instancia local de PostgreSQL 16+)

### 1. Iniciar la Base de Datos
Puedes levantar el contenedor de PostgreSQL incluido con Docker Compose:
```bash
docker compose up -d
```

### 2. Configuración
Verifica o edita la cadena de conexión en [src/ProjectTaskManager.API/appsettings.json](src/ProjectTaskManager.API/appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=projecttaskmanager;Username=postgres;Password=postgres"
  }
}
```

### 3. Aplicar Migraciones
Ejecuta las migraciones de EF Core para generar la estructura en la base de datos:
```bash
dotnet ef database update --project src/ProjectTaskManager.Infrastructure --startup-project src/ProjectTaskManager.API
```

### 4. Ejecutar la API
```bash
dotnet run --project src/ProjectTaskManager.API
```

### 5. Documentación Swagger UI
Abre tu navegador web y navega a cualquiera de las siguientes URLs:
- **HTTPS:** [https://localhost:7042/swagger](https://localhost:7042/swagger)
- **HTTP:** [http://localhost:5230/swagger](http://localhost:5230/swagger)
