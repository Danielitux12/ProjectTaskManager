using ProjectTaskManager.Application.DTOs;
using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.Application.Interfaces;

public interface ITareaService
{
    Task<IEnumerable<TareaDto>> ObtenerPorProyectoAsync(Guid proyectoId);
    Task<IEnumerable<TareaDto>> ObtenerConFiltrosAsync(EstadoTarea? estado, Prioridad? prioridad);
    Task<IEnumerable<TareaDto>> ObtenerVencidasAsync();
    Task<TareaDto> ObtenerPorIdAsync(Guid id);
    Task<TareaDto> CrearAsync(CrearTareaDto dto);
    Task ActualizarAsync(Guid id, CrearTareaDto dto);
    Task CambiarEstadoAsync(Guid id, EstadoTarea nuevoEstado);
    Task EliminarAsync(Guid id);
}