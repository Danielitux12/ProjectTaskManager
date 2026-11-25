using ProjectTaskManager.Application.DTOs;

namespace ProjectTaskManager.Application.Interfaces;

public interface IProyectoService
{
    Task<IEnumerable<ProyectoDto>> ObtenerTodosAsync();
    Task<ProyectoDto> ObtenerPorIdAsync(Guid id);
    Task<ProyectoDto> CrearAsync(CrearProyectoDto dto);
    Task ActualizarAsync(Guid id, CrearProyectoDto dto);
    Task EliminarAsync(Guid id);
    Task<decimal> CalcularAvanceAsync(Guid id);
}