namespace ProjectTaskManager.Application.DTOs;

public record ProyectoDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin
);

public record CrearProyectoDto(
    string Nombre,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin
);