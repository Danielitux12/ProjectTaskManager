using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.Application.DTOs;

public record TareaDto(
    Guid Id,
    string Titulo,
    string? Descripcion,
    EstadoTarea Estado,
    Prioridad Prioridad,
    DateTime FechaLimite,
    Guid ProyectoId
);

public record CrearTareaDto(
    string Titulo,
    string? Descripcion,
    Prioridad Prioridad,
    DateTime FechaLimite,
    Guid ProyectoId
);

public record CambiarEstadoTareaDto(EstadoTarea NuevoEstado);
