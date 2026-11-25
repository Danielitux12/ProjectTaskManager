using ProjectTaskManager.Application.DTOs;
using ProjectTaskManager.Application.Exceptions;
using ProjectTaskManager.Application.Interfaces;
using ProjectTaskManager.Domain.Entities;
using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.Application.Services;

public class ProyectoService(IProyectoRepository repositorio) : IProyectoService
{
    public async Task<IEnumerable<ProyectoDto>> ObtenerTodosAsync()
    {
        var proyectos = await repositorio.ObtenerTodosAsync();
        return proyectos.Select(MapearADto);
    }

    public async Task<ProyectoDto> ObtenerPorIdAsync(Guid id)
    {
        var proyecto = await repositorio.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con id {id}");
        return MapearADto(proyecto);
    }

    public async Task<ProyectoDto> CrearAsync(CrearProyectoDto dto)
    {
        var proyecto = new Proyecto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin
        };

        await repositorio.AgregarAsync(proyecto);
        await repositorio.GuardarCambiosAsync();

        return MapearADto(proyecto);
    }

    public async Task ActualizarAsync(Guid id, CrearProyectoDto dto)
    {
        var proyecto = await repositorio.ObtenerPorIdAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con id {id}");

        proyecto.Nombre = dto.Nombre;
        proyecto.Descripcion = dto.Descripcion;
        proyecto.FechaInicio = dto.FechaInicio;
        proyecto.FechaFin = dto.FechaFin;

        repositorio.Actualizar(proyecto);
        await repositorio.GuardarCambiosAsync();
    }

    public async Task EliminarAsync(Guid id)
    {
        var proyecto = await repositorio.ObtenerConTareasAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con id {id}");

        // Requerimiento: impedir eliminación con tareas pendientes
        var tieneTareasPendientes = proyecto.Tareas.Any(t => t.Estado != EstadoTarea.Completada);
        if (tieneTareasPendientes)
            throw new BusinessRuleException(
                "No se puede eliminar el proyecto: tiene tareas sin completar.");

        repositorio.Eliminar(proyecto);
        await repositorio.GuardarCambiosAsync();
    }

    public async Task<decimal> CalcularAvanceAsync(Guid id)
    {
        var proyecto = await repositorio.ObtenerConTareasAsync(id)
            ?? throw new NotFoundException($"No se encontró el proyecto con id {id}");

        if (proyecto.Tareas.Count == 0)
            return 0m;

        var completadas = proyecto.Tareas.Count(t => t.Estado == EstadoTarea.Completada);
        return Math.Round((decimal)completadas / proyecto.Tareas.Count * 100, 2);
    }

    private static ProyectoDto MapearADto(Proyecto p) =>
        new(p.Id, p.Nombre, p.Descripcion, p.FechaInicio, p.FechaFin);
}