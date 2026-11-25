using ProjectTaskManager.Domain.Entities;
using ProjectTaskManager.Domain.Enums;

namespace ProjectTaskManager.Application.Interfaces;

public interface ITareaRepository
{
    Task<IEnumerable<Tarea>> ObtenerPorProyectoAsync(Guid proyectoId);
    Task<IEnumerable<Tarea>> ObtenerConFiltrosAsync(EstadoTarea? estado, Prioridad? prioridad);
    Task<IEnumerable<Tarea>> ObtenerVencidasAsync();
    Task<Tarea?> ObtenerPorIdAsync(Guid id);
    Task AgregarAsync(Tarea tarea);
    void Actualizar(Tarea tarea);
    void Eliminar(Tarea tarea);
    Task<bool> GuardarCambiosAsync();
}