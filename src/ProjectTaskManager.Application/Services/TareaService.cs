using ProjectTaskManager.Application.DTOs;
using ProjectTaskManager.Application.Exceptions;
using ProjectTaskManager.Application.Interfaces;
using ProjectTaskManager.Domain.Entities;
using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.Application.Services;

public class TareaService(ITareaRepository repositorio, IProyectoRepository proyectoRepositorio) : ITareaService
{
    public async Task<IEnumerable<TareaDto>> ObtenerPorProyectoAsync(Guid proyectoId)
    {
        var tareas = await repositorio.ObtenerPorProyectoAsync(proyectoId);
        return tareas.Select(MapearADto);
    }

    public async Task<IEnumerable<TareaDto>> ObtenerConFiltrosAsync(EstadoTarea? estado, Prioridad? prioridad)
    {
        var tareas = await repositorio.ObtenerConFiltrosAsync(estado, prioridad);
        return tareas.Select(MapearADto);
    }

    public async Task<IEnumerable<TareaDto>> ObtenerVencidasAsync()
    {
        var tareas = await repositorio.ObtenerVencidasAsync();
        return tareas.Select(MapearADto);
    }

    public async Task<TareaDto> ObtenerPorIdAsync(Guid id)
    {
        var tarea = await repositorio.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con id {id}");
        return MapearADto(tarea);
    }

    public async Task<TareaDto> CrearAsync(CrearTareaDto dto)
    {
        // Validar que el proyecto exista antes de crear la tarea
        _ = await proyectoRepositorio.ObtenerPorIdAsync(dto.ProyectoId)
            ?? throw new NotFoundException($"No se encontró el proyecto con id {dto.ProyectoId}");

        var tarea = new Tarea
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Prioridad = dto.Prioridad,
            FechaLimite = dto.FechaLimite,
            ProyectoId = dto.ProyectoId
            // Estado queda en el default de la entidad: Pendiente
        };

        await repositorio.AgregarAsync(tarea);
        await repositorio.GuardarCambiosAsync();

        return MapearADto(tarea);
    }

    public async Task ActualizarAsync(Guid id, CrearTareaDto dto)
    {
        var tarea = await repositorio.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con id {id}");

        tarea.Titulo = dto.Titulo;
        tarea.Descripcion = dto.Descripcion;
        tarea.Prioridad = dto.Prioridad;
        tarea.FechaLimite = dto.FechaLimite;

        repositorio.Actualizar(tarea);
        await repositorio.GuardarCambiosAsync();
    }

    public async Task CambiarEstadoAsync(Guid id, EstadoTarea nuevoEstado)
    {
        var tarea = await repositorio.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con id {id}");

        tarea.Estado = nuevoEstado;

        repositorio.Actualizar(tarea);
        await repositorio.GuardarCambiosAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var tarea = await repositorio.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException($"No se encontró la tarea con id {id}");

        repositorio.Eliminar(tarea);
        await repositorio.GuardarCambiosAsync();
    }

    private static TareaDto MapearADto(Tarea t) =>
        new(t.Id, t.Titulo, t.Descripcion, t.Estado, t.Prioridad, t.FechaLimite, t.ProyectoId);
}